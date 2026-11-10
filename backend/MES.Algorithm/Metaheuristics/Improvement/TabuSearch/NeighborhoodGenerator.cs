using MesMicroservice.Algorithm.Heuristics.Assignment;
using System.Diagnostics;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using static MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch.Move;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch
{
    public class NeighborhoodGenerator
    {
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IAssignmentMethod _assignment;
        private readonly SetupTransitionCache _setupCache;

        public NeighborhoodGenerator(
            IMachineTimeCalculator machineTimeCalculator,
            IAssignmentMethod assignmentMethod,
            SetupTransitionCache setupCache)
        {
            _machineTimeCalculator = machineTimeCalculator;
            _assignment = assignmentMethod;
            _setupCache = setupCache;
        }

        private bool IsBadTransition(Job prev, Job current)
        {
            if (prev == null || current == null) return false;
            return _setupCache.Get(prev.Id, current.Id) > 0;
        }

        private List<Job> GetSetupConflictJobs(Schedule schedule)
        {
            var result = new List<Job>();

            var jobsByMachine = schedule.Jobs.Values
                .SelectMany(j => j)
                .Where(j => !string.IsNullOrEmpty(j.AssignedMachineCode))
                .GroupBy(j => j.AssignedMachineCode);

            foreach (var machineJobs in jobsByMachine)
            {
                var orderedJobs = machineJobs
                    .OrderBy(j => j.StartTime)
                    .ThenBy(j => j.IndexInSchedule)
                    .ToList();

                for (int i = 1; i < orderedJobs.Count; i++)
                {
                    if (IsBadTransition(orderedJobs[i - 1], orderedJobs[i]))
                        result.Add(orderedJobs[i]);
                }
            }

            return result
                .GroupBy(j => j.Id)
                .Select(g => g.First())
                .ToList();
        }

        private List<Job> GetCriticalJobs(Schedule schedule, Machine bottleneckMachine)
        {
            return schedule.Jobs.Values.SelectMany(j => j)
                .Where(j => j.AssignedMachineCode == bottleneckMachine.Code)
                .OrderByDescending(j => j.EndTime).ToList();
        }

        private Machine? GetBottleneckMachine(Schedule schedule)
        {
            if (schedule.Machines == null || !schedule.Machines.Any())
                return null;

            return schedule.Machines
                .Select(m => new
                {
                    Machine = m,
                    Load = schedule.Jobs.Values
                        .SelectMany(j => j)
                        .Where(j => j.AssignedMachineCode == m.Code)
                        .Select(j => j.EndTime)
                        .DefaultIfEmpty(0)
                        .Max()
                })
                .OrderByDescending(x => x.Load)
                .FirstOrDefault()
                ?.Machine;
        }

        public async Task<List<(Schedule, Move)>> GenerateNeighbors_SwapIndex(
    Schedule schedule,
    int numNeighbors,
    Random random)
        {
            var neighbors = new List<(Schedule, Move)>();

            if (schedule?.Jobs == null || schedule.Jobs.Count == 0)
                return neighbors;

            var allJobs = schedule.Jobs.Values
                .SelectMany(j => j)
                .OrderBy(j => j.IndexInSchedule)
                .ThenBy(j => j.Id)
                .ToList();

            if (allJobs.Count <= 1)
                return neighbors;

            var tardyJobs = allJobs
                .Where(j => j.DueDate > 0 && j.EndTime > j.DueDate)
                .OrderByDescending(j => j.EndTime - j.DueDate)
                .ToList();

            var usedMoves = new HashSet<string>();
            int attempts = 0;
            int maxAttempts = 1000 + numNeighbors * 80;
            int window = Math.Max(5, allJobs.Count / 3);

            while (neighbors.Count < numNeighbors && attempts++ < maxAttempts)
            {
                Job originalA = PickJobA(allJobs, tardyJobs, random);

                var candidatesB = allJobs
                    .Where(j => j.Id != originalA.Id)
                    .Where(j =>
                        Math.Abs(j.IndexInSchedule - originalA.IndexInSchedule) <= window ||
                        tardyJobs.Any(t => t.Id == j.Id))
                    .ToList();

                if (!candidatesB.Any())
                    continue;

                var originalB = candidatesB[random.Next(candidatesB.Count)];

                int oldIndexA = originalA.IndexInSchedule;
                int oldIndexB = originalB.IndexInSchedule;

                string moveKey = $"S:{Math.Min(originalA.Id, originalB.Id)}:{Math.Max(originalA.Id, originalB.Id)}";
                if (!usedMoves.Add(moveKey))
                    continue;

                var testJobs = allJobs.Select(j => j.Clone()).ToList();
                var testA = testJobs.First(j => j.Id == originalA.Id);
                var testB = testJobs.First(j => j.Id == originalB.Id);

                testA.IndexInSchedule = oldIndexB;
                testB.IndexInSchedule = oldIndexA;
                NormalizeIndex(testJobs);

                if (ViolatesSequenceInMO(testJobs))
                    continue;

                var clone = DeepClone(schedule);
                var jobsClone = clone.Jobs.Values.SelectMany(j => j).ToList();
                var jobById = jobsClone.ToDictionary(j => j.Id);

                var jobA = jobById[originalA.Id];
                var jobB = jobById[originalB.Id];

                jobA.IndexInSchedule = oldIndexB;
                jobB.IndexInSchedule = oldIndexA;
                NormalizeIndex(jobsClone);

                clone.Jobs = jobsClone
                    .GroupBy(j => j.ManufacturingOrderId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var solution = await _assignment.Run(clone);

                neighbors.Add((solution.BestSchedule, new Move
                {
                    MoIdA = jobA.ManufacturingOrderId,
                    JobAId = jobA.Id,
                    StageA = jobA.SequenceNumber,
                    MoIdB = jobB.ManufacturingOrderId,
                    JobBId = jobB.Id,
                    StageB = jobB.SequenceNumber,
                    OldIndexA = oldIndexA,
                    OldIndexB = oldIndexB,
                    NewIndexA = jobA.IndexInSchedule,
                    NewIndexB = jobB.IndexInSchedule
                }));
            }

            return neighbors;
        }

        public async Task<List<(Schedule, Move)>> GenerateNeighbors_Insert(
    Schedule schedule,
    int numNeighbors,
    Dictionary<int, int> moveFrequency,
    Random random)
        {
            var neighbors = new List<(Schedule, Move)>();

            if (schedule?.Jobs == null || schedule.Jobs.Count == 0)
                return neighbors;

            var allJobs = schedule.Jobs.Values
                .SelectMany(j => j)
                .OrderBy(j => j.IndexInSchedule)
                .ThenBy(j => j.Id)
                .ToList();

            if (allJobs.Count <= 2)
                return neighbors;

            var tardyJobs = allJobs
                .Where(j => j.DueDate > 0 && j.EndTime > j.DueDate)
                .OrderByDescending(j => j.EndTime - j.DueDate)
                .ToList();

            var bottleneckMachine = GetBottleneckMachine(schedule);
            var criticalJobs = bottleneckMachine != null
                ? GetCriticalJobs(schedule, bottleneckMachine)
                : new List<Job>();

            var setupConflictJobs = GetSetupConflictJobs(schedule);
            var usedMoves = new HashSet<string>();

            int attempts = 0;
            int maxAttempts = 1000 + numNeighbors * 80;

            while (neighbors.Count < numNeighbors && attempts++ < maxAttempts)
            {
                Job selectedJob;
                double strategy = random.NextDouble();

                if (criticalJobs.Any() && strategy < 0.25)
                {
                    selectedJob = criticalJobs
                        .OrderBy(j => moveFrequency.ContainsKey(j.Id) ? moveFrequency[j.Id] : 0)
                        .Take(Math.Min(8, criticalJobs.Count))
                        .OrderBy(_ => random.Next())
                        .First();
                }
                else if (setupConflictJobs.Any() && strategy < 0.50)
                {
                    selectedJob = setupConflictJobs
                        .OrderBy(j => moveFrequency.ContainsKey(j.Id) ? moveFrequency[j.Id] : 0)
                        .Take(Math.Min(8, setupConflictJobs.Count))
                        .OrderBy(_ => random.Next())
                        .First();
                }
                else if (tardyJobs.Any() && strategy < 0.75)
                {
                    selectedJob = tardyJobs[random.Next(Math.Min(8, tardyJobs.Count))];
                }
                else
                {
                    selectedJob = allJobs[random.Next(allJobs.Count)];
                }

                int oldIndex = selectedJob.IndexInSchedule;
                int newIndex;

                var compatibleIndexes = allJobs
                    .Where(j => j.Id != selectedJob.Id)
                    .Where(j =>
                        SameMaterialSpec(j, selectedJob) &&
                        Math.Abs(j.Diameter - selectedJob.Diameter) < 0.01)
                    .Select(j => j.IndexInSchedule)
                    .ToList();

                if (compatibleIndexes.Any() && random.NextDouble() < 0.55)
                {
                    int anchor = compatibleIndexes[random.Next(compatibleIndexes.Count)];
                    newIndex = Math.Clamp(anchor + (random.Next(2) == 0 ? 0 : 1), 0, allJobs.Count - 1);
                }
                else if (random.NextDouble() < 0.35)
                {
                    newIndex = random.Next(allJobs.Count);
                }
                else
                {
                    int range = Math.Max(10, allJobs.Count / 3);
                    int minIndex = Math.Max(0, oldIndex - range);
                    int maxIndex = Math.Min(allJobs.Count - 1, oldIndex + range);
                    newIndex = random.Next(minIndex, maxIndex + 1);
                }

                if (newIndex == oldIndex)
                    continue;

                string moveKey = $"I:{selectedJob.Id}:{oldIndex}->{newIndex}";
                if (!usedMoves.Add(moveKey))
                    continue;

                var clone = DeepClone(schedule);
                var jobsClone = clone.Jobs.Values
                    .SelectMany(j => j)
                    .OrderBy(j => j.IndexInSchedule)
                    .ThenBy(j => j.Id)
                    .ToList();

                var movingJob = jobsClone.First(j => j.Id == selectedJob.Id);

                jobsClone.Remove(movingJob);
                newIndex = Math.Clamp(newIndex, 0, jobsClone.Count);
                jobsClone.Insert(newIndex, movingJob);
                NormalizeIndex(jobsClone);

                if (ViolatesSequenceInMO(jobsClone))
                    continue;

                clone.Jobs = jobsClone
                    .GroupBy(j => j.ManufacturingOrderId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var solution = await _assignment.Run(clone);

                neighbors.Add((solution.BestSchedule, new Move
                {
                    Type = MoveType.Insert,
                    JobAId = movingJob.Id,
                    MoIdA = movingJob.ManufacturingOrderId,
                    StageA = movingJob.SequenceNumber,
                    OldIndexA = oldIndex,
                    NewIndexA = newIndex
                }));
            }

            return neighbors;
        }

        private bool ViolatesSequenceInMO(List<Job> jobs)
        {
            foreach (var g in jobs.GroupBy(j => j.ManufacturingOrderId))
            {
                int maxSeq = -1;
                foreach (var j in g.OrderBy(x => x.IndexInSchedule))
                {
                    int seq = j.SequenceNumber ?? 0;
                    if (seq < maxSeq) return true;
                    maxSeq = seq;
                }
            }
            return false;
        }

        private bool CanSwapSequenceNumberSafe(Job a, Job b)
        {
            if (a.ManufacturingOrderId != b.ManufacturingOrderId)
                return true;

            int seqA = a.SequenceNumber ?? 0;
            int seqB = b.SequenceNumber ?? 0;

            if (seqA == seqB)
                return true;

            // VỊ TRÍ SAU KHI SWAP
            int newPosA = b.IndexInSchedule;
            int newPosB = a.IndexInSchedule;

            // Công đoạn trước không được đứng sau công đoạn sau
            if (seqA < seqB && newPosA > newPosB)
                return false;

            if (seqA > seqB && newPosA < newPosB)
                return false;

            return true;
        }

        private static Job PickJobA(List<Job> allJobs, List<Job> tardyJobs, Random random)
        {
            if (tardyJobs.Any() && random.NextDouble() < 0.7)
                return tardyJobs[random.Next(Math.Min(8, tardyJobs.Count))];

            return allJobs[random.Next(allJobs.Count)];
        }
        private static bool SameMaterialSpec(Job a, Job b)
        {
            var aSpecs = a.MaterialSpecInfors ?? new List<string>();
            var bSpecs = b.MaterialSpecInfors ?? new List<string>();

            return new HashSet<string>(aSpecs).SetEquals(bSpecs);
        }

        private static void NormalizeIndex(List<Job> jobs)
        {
            jobs.OrderBy(j => j.IndexInSchedule).ThenBy(j => j.Id).Select((j, i) => (j, i)).ToList().ForEach(x => x.j.IndexInSchedule = x.i);
        }

        private Schedule DeepClone(Schedule sch)
        {
            var clonedMachines = sch.Machines.Select(m => m.Clone()).ToList();
            var clonedJobs = sch.Jobs.ToDictionary(kv => kv.Key, kv => kv.Value.OrderBy(j => j.IndexInSchedule).Select(j => j.Clone()).ToList());
            return new Schedule(clonedMachines, clonedJobs);
        }
    }
}