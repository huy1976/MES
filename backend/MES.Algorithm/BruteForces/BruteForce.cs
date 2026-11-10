using MesMicroservice.Algorithm.Heuristics.Assignment;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace MesMicroservice.Algorithm.BruteForces
{
    public class BruteForce : IAlgorithm
    {
        private readonly IAssignmentMethod _assignment;
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IEquipmentSelectorService _equipmentSelectorService;
        private readonly ObjectiveEvaluator evaluator = new ObjectiveEvaluator();

        public BruteForce(IAssignmentMethod assignment,
                          IMachineTimeCalculator machineTimeCalculator,
                          IEquipmentSelectorService equipmentSelectorService)
        {
            _assignment = assignment;
            _machineTimeCalculator = machineTimeCalculator;
            _equipmentSelectorService = equipmentSelectorService;
        }

        public async Task<Solution> Run(Schedule schedule)
        {
            var stopwatch = Stopwatch.StartNew();

            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            var jobs = schedule.Jobs ?? throw new ArgumentException("Jobs cannot be null.", nameof(schedule));

            await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(jobs);
            _machineTimeCalculator.CalculateProcessingTimes(jobs);

            // === 1. Collect all jobs ===
            var allJobs = schedule.Jobs.Values
                .SelectMany(j => j)
                .OrderBy(j => j.IndexInSchedule)
                .ThenBy(j => j.Id)
                .ToList();

            if (!allJobs.Any())
                return new Solution { BestSchedule = schedule };

            Schedule? bestSchedule = null;
            double bestObjective = double.MaxValue;

            int iteration = 0;

            using var logger = new CsvLoggerExtended("BruteForce");

            // === 2. Generate and Evaluate valid permutations (LAZY EVALUATION) ===
            foreach (var permutation in GenerateValidPermutationsLazy(allJobs, 0))
            {
                iteration++;

                // === 3. Clone schedule (Đã sửa lỗi dính tham chiếu Máy) ===
                var clone = DeepClone(schedule);
                var jobsClone = clone.Jobs.Values.SelectMany(j => j).ToList();
                var jobById = jobsClone.ToDictionary(j => j.Id);

                // === 4. Apply permutation ===
                for (int i = 0; i < permutation.Count; i++)
                {
                    jobById[permutation[i].Id].IndexInSchedule = i;
                }

                // === 5. Decode ===
                var solution = await _assignment.Run(clone);
                var currentSchedule = solution.BestSchedule;

                // [ĐÃ SỬA] Đổi sang dùng Evaluate() chuẩn mực
                double currentObjective = evaluator.Evaluate(currentSchedule);

                // === 6. Update best ===
                if (currentObjective < bestObjective)
                {
                    bestObjective = currentObjective;
                    bestSchedule = currentSchedule.Clone();
                }

                // === 7. CSV LOG ===
                logger.Log(
                    iteration,
                    evaluator.CalculateMakespan(currentSchedule),
                    bestSchedule != null ? evaluator.CalculateMakespan(bestSchedule) : 0,

                    evaluator.CalculateTotalLateness(currentSchedule),
                    bestSchedule != null ? evaluator.CalculateTotalLateness(bestSchedule) : 0,

                    evaluator.CalculateTotalSetupTime(currentSchedule),
                    bestSchedule != null ? evaluator.CalculateTotalSetupTime(bestSchedule) : 0,

                    0, // penalty 
                    0,

                    currentSchedule != null ? evaluator.Evaluate(currentSchedule) : 0, // [ĐÃ SỬA]
                    bestSchedule != null ? evaluator.Evaluate(bestSchedule) : 0,       // [ĐÃ SỬA]
                    stopwatch.Elapsed.TotalSeconds
                );
            }

            stopwatch.Stop();

            if (bestSchedule == null)
                throw new InvalidOperationException("No valid solution found in brute-force search.");

            // === FINAL RESULT ===
            return new Solution
            {
                BestSchedule = bestSchedule,
                BestObjective = bestObjective,
                Runtime = stopwatch.Elapsed,
                Iterations = iteration,
                Makespan = evaluator.CalculateMakespan(bestSchedule),
                TotalSetupTime = evaluator.CalculateTotalSetupTime(bestSchedule),
                TotalLateness = evaluator.CalculateTotalLateness(bestSchedule),
                Metaheuristic = EAlgorithmType.BruteForce.ToString(),
                GeneratedAt = DateTime.UtcNow
            };
        }

        // =========================================================
        // =============== PERMUTATION GENERATOR (LAZY) ============
        // =========================================================

        // [ĐÃ SỬA] Hàm sinh hoán vị siêu nhẹ RAM, dùng yield return
        private IEnumerable<List<Job>> GenerateValidPermutationsLazy(List<Job> jobs, int index)
        {
            if (index == jobs.Count)
            {
                if (!ViolatesSequenceInMO(jobs))
                    yield return jobs.ToList();
                yield break;
            }

            for (int i = index; i < jobs.Count; i++)
            {
                Swap(jobs, index, i);

                if (!ViolatesSequencePrefix(jobs, index))
                {
                    // Lướt qua từng cấu hình lân cận và nhả (yield) ngay ra ngoài cho vòng lặp chạy
                    foreach (var p in GenerateValidPermutationsLazy(jobs, index + 1))
                    {
                        yield return p;
                    }
                }

                // Quay lui (Backtrack)
                Swap(jobs, index, i);
            }
        }

        // =========================================================
        // ================== CONSTRAINT CHECK =====================
        // =========================================================

        private bool ViolatesSequencePrefix(List<Job> jobs, int upto)
        {
            var dict = new Dictionary<string, int>();

            for (int i = 0; i <= upto; i++)
            {
                var j = jobs[i];
                int seq = j.SequenceNumber ?? 0;

                if (!dict.TryGetValue(j.ManufacturingOrderId, out int last))
                {
                    dict[j.ManufacturingOrderId] = seq;
                }
                else
                {
                    if (seq < last)
                        return true;

                    dict[j.ManufacturingOrderId] = seq;
                }
            }
            return false;
        }

        private bool ViolatesSequenceInMO(List<Job> jobs)
        {
            foreach (var g in jobs.GroupBy(j => j.ManufacturingOrderId))
            {
                int maxSeq = -1;
                foreach (var j in g)
                {
                    int seq = j.SequenceNumber ?? 0;
                    if (seq < maxSeq)
                        return true;
                    maxSeq = seq;
                }
            }
            return false;
        }

        // =========================================================
        // ================== UTILS ================================
        // =========================================================

        private static void Swap(List<Job> list, int i, int j)
        {
            if (i == j) return;
            (list[i], list[j]) = (list[j], list[i]);
        }

        private Schedule DeepClone(Schedule sch)
        {
            // [ĐÃ SỬA LỖI] BẮT BUỘC PHẢI CLONE CẢ DANH SÁCH MÁY MÓC
            var clonedMachines = sch.Machines.Select(m => m.Clone()).ToList();

            var clonedJobs = sch.Jobs.ToDictionary(
                kv => kv.Key,
                kv => kv.Value
                    .OrderBy(j => j.IndexInSchedule)
                    .Select(j => j.Clone())
                    .ToList()
            );

            return new Schedule(clonedMachines, clonedJobs);
        }
    }
}