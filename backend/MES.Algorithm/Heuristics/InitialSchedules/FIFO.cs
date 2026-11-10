using MesMicroservice.Algorithm.Heuristics.Assignment;
using System.Diagnostics;

namespace MesMicroservice.Algorithm.Heuristics.InitialSchedules
{
    public class FIFO : IHeuristic
    {
        private readonly IEquipmentSelectorService _equipmentSelectorService;
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IAssignmentMethod _assignmentMethod;
        private readonly ObjectiveEvaluator evaluator = new ObjectiveEvaluator();
        public FIFO(IEquipmentSelectorService equipmentSelectorService, IMachineTimeCalculator machineTimeCalculator, IAssignmentMethod assignmentMethod)
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

            // Sắp xếp FIFO
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
                Metaheuristic = EAlgorithmType.FIFO.ToString(),
                GeneratedAt = DateTime.UtcNow
            };
        }

        private Dictionary<string, List<Job>> GenerateInitialOrder(Dictionary<string, List<Job>> jobs)
        {
            // Gom tất cả job thành 1 list
            var allJobs = jobs.SelectMany(kv => kv.Value)
                              .Where(j => j != null)
                              .ToList();

            if (!allJobs.Any()) return jobs; // fallback

            // Sắp xếp theo ManufacturingOrderId và StepNumber để đảm bảo thứ tự công đoạn
            var orderedJobs = allJobs
                .OrderBy(j => j.ManufacturingOrderId)       // FIFO theo MO   // Đảm bảo thứ tự công đoạn
                .ThenBy(j => j.SequenceNumber ?? 0)        // Optional: nếu trong step có nhiều job
                .ToList();

            // Gán IndexInSchedule theo thứ tự FIFO
            for (int i = 0; i < orderedJobs.Count; i++)
            {
                orderedJobs[i].IndexInSchedule = i;
            }

            // Nhóm lại theo MO
            var grouped = orderedJobs.GroupBy(j => j.ManufacturingOrderId)
                                     .ToDictionary(g => g.Key, g => g.ToList());

            return grouped;
        }

    }
}
