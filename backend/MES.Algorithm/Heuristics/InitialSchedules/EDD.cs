using MesMicroservice.Algorithm.Heuristics.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MesMicroservice.Algorithm.Heuristics.InitialSchedules
{
    public class EDD : IHeuristic
    {
        private readonly IEquipmentSelectorService _equipmentSelectorService;
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IAssignmentMethod _assignmentMethod;
        private readonly ObjectiveEvaluator evaluator = new ObjectiveEvaluator();

        public EDD(IEquipmentSelectorService equipmentSelectorService, IMachineTimeCalculator machineTimeCalculator, IAssignmentMethod assignmentMethod)
        {
            _equipmentSelectorService = equipmentSelectorService;
            _machineTimeCalculator = machineTimeCalculator;
            _assignmentMethod = assignmentMethod;
        }

        public async Task<Solution> Run(Schedule schedule)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            var jobs = schedule.Jobs ?? throw new ArgumentException("Jobs cannot be null.", nameof(schedule));

            // Gán máy và tính thời gian
            await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(jobs);
            _machineTimeCalculator.CalculateProcessingTimes(jobs);

            // Sắp xếp EDD
            var orderedJobs = GenerateInitialOrder(jobs);
            schedule.Jobs = orderedJobs;

            var solution = await _assignmentMethod.Run(schedule);

            return new Solution
            {
                BestSchedule = solution.BestSchedule,
                BestObjective = evaluator.Evaluate(solution.BestSchedule),
                Makespan = evaluator.CalculateMakespan(solution.BestSchedule),
                TotalSetupTime = evaluator.CalculateTotalSetupTime(solution.BestSchedule),
                TotalLateness = evaluator.CalculateTotalLateness(solution.BestSchedule),
                Metaheuristic = EAlgorithmType.EDD.ToString(),
                GeneratedAt = DateTime.UtcNow
            };
        }

        private Dictionary<string, List<Job>> GenerateInitialOrder(Dictionary<string, List<Job>> jobs)
        {
            var allJobs = jobs.SelectMany(kv => kv.Value)
                              .Where(j => j != null)
                              .ToList();

            if (!allJobs.Any()) return jobs; // fallback

            // Sắp xếp theo DueDate (Earliest Due Date) và SequenceNumber
            var orderedJobs = allJobs
                .OrderBy(j => j.DueDate)          // DueDate là double
                .ThenBy(j => j.SequenceNumber ?? 0)
                .ToList();

            for (int i = 0; i < orderedJobs.Count; i++)
            {
                orderedJobs[i].IndexInSchedule = i;
            }

            var grouped = orderedJobs.GroupBy(j => j.ManufacturingOrderId)
                                     .ToDictionary(g => g.Key, g => g.ToList());

            return grouped;
        }

    }
}
