namespace MesMicroservice.Algorithm.Heuristics.Assignment;

public class Assignment : IAssignmentMethod
{
    private readonly IMachineTimeCalculator _machineTimeCalculator;
    private readonly ObjectiveEvaluator _objectiveEvaluator = new();
    private readonly Random _rand = new();

    private const double NoiseLevel = 0.05;

    public Assignment(IMachineTimeCalculator machineTimeCalculator)
    {
        _machineTimeCalculator = machineTimeCalculator;
    }

    // ================= ENTRY =================

    public Task<Solution> Run(Schedule originalSchedule)
    {
        // Gom job theo stage (SequenceNumber)
        var stagesMap = originalSchedule.Jobs
            .SelectMany(kv => kv.Value)
            .GroupBy(j => j.SequenceNumber ?? 1)
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(j => j.IndexInSchedule).ToList()
            );

        var stageStrategies = stagesMap.Keys
            .ToDictionary(k => k, _ => AssignmentStrategy.MinSetup);

        var solution = GenerateSolution(originalSchedule, stagesMap, stageStrategies);
        return Task.FromResult(solution);
    }

    // ================= CORE DECODER =================

    private Solution GenerateSolution(
        Schedule originalSchedule,
        Dictionary<int, List<Job>> stagesMap,
        Dictionary<int, AssignmentStrategy> strategies)
    {
        var stageDeadlines = BuildStageDeadlines(stagesMap);

        // Clone machines (reset state)
        var newMachines = originalSchedule.Machines
            .Select(CloneMachine)
            .ToList();


        var newSchedule = new Schedule(
            newMachines,
            new Dictionary<string, List<Job>>()
        );

        foreach (var stage in stagesMap.Keys.OrderBy(k => k))
        {
            foreach (var originalJob in stagesMap[stage])
            {
                var job = originalJob.Clone();

                if (!newSchedule.Jobs.ContainsKey(job.ManufacturingOrderId))
                    newSchedule.Jobs[job.ManufacturingOrderId] = new List<Job>();

                double moReadyTime = GetReadyTimeFromPreviousSequence(newSchedule, job);

                var eligibleMachines = newMachines
                    .Where(m => job.EligibleMachines.Any(em => em.Code == m.Code))
                    .ToList();

                double stageDeadline = stageDeadlines[(job.ManufacturingOrderId, job.SequenceNumber ?? 1)];

                AssignJobToBestMachine(
                    job,
                    eligibleMachines,
                    newSchedule,
                    moReadyTime,
                    stageDeadline
                );
            }
        }

        return FinalizeSchedule(newSchedule);
    }

    // ================= ASSIGNMENT =================

    private void AssignJobToBestMachine(
        Job job,
        List<Machine> machines,
        Schedule schedule,
        double moReadyTime,
        double stageDeadline)
    {
        if (machines == null || !machines.Any())
        {
            throw new Exception($"[Lỗi phân lịch] Lệnh sản xuất '{job.ManufacturingOrderId}' (Công đoạn {job.SequenceNumber}) không có bất kỳ máy nào sẵn sàng để chạy.");
        }

        Machine? bestMachine = null;
        double bestEndTime = double.MaxValue;
        double bestSetup = double.MaxValue;

        // Khôi phục trạng thái PreviousJob nếu máy chưa chạy lệnh nào
        foreach (var m in machines)
        {
            var cap = job.EligibleMachines.First(e => e.Code == m.Code);
            if (m.PreviousJob is null && cap.PreviousJob is not null)
            {
                m.PreviousJob = cap.PreviousJob;
                m.AvailableTime = cap.AvailableTime;
            }
        }
        foreach (var m in machines)
        {
            var cap = job.EligibleMachines.First(e => e.Code == m.Code);

            double setup = CalculateSetupTime(m, job);
            double start = Math.Max(
                Math.Max(m.TotalTime, m.AvailableTime),
                moReadyTime
            );

            double end = start + setup + cap.ProcessingTime;
            if (end < bestEndTime || (Math.Abs(end - bestEndTime) < 0.001 && setup < bestSetup))
            {
                bestEndTime = end;
                bestSetup = setup;
                bestMachine = m;
            }
        }

        // Lúc này bestMachine chắc chắn đã lưu cỗ máy hoàn hảo nhất
        PlaceJobOnMachine(job, bestMachine!, schedule, moReadyTime);
    }


    // ================= TIME =================

    private Dictionary<(string moId, int stage), double> BuildStageDeadlines(
        Dictionary<int, List<Job>> stagesMap)
    {
        var result = new Dictionary<(string, int), double>();

        var mos = stagesMap
            .SelectMany(kv => kv.Value)
            .GroupBy(j => j.ManufacturingOrderId);

        foreach (var mo in mos)
        {
            var sequenceGroups = mo
                .Where(j => j.SequenceNumber.HasValue)
                .GroupBy(j => j.SequenceNumber!.Value)
                .OrderByDescending(g => g.Key)
                .ToList();

            if (!sequenceGroups.Any())
                continue;

            double currentDeadline = sequenceGroups.First().First().DueDate;

            Job? fromJob = null;

            for (int i = 0; i < sequenceGroups.Count; i++)
            {
                var group = sequenceGroups[i];
                int sequence = group.Key;

                // 1️⃣ Gán deadline cho toàn bộ job cùng sequence
                foreach (var job in group)
                {
                    result[(job.ManufacturingOrderId, sequence)] = currentDeadline;
                }

                // 2️⃣ Trừ deadline để dùng cho sequence tiếp theo
                if (i < sequenceGroups.Count - 1)
                {
                    double maxProc = group
                        .SelectMany(j => j.EligibleMachines)
                        .Max(m => m.ProcessingTime);

                    double maxSetup = group
                        .SelectMany(j => j.EligibleMachines)
                        .Max(m =>
                            _machineTimeCalculator.SetupTime(
                                fromJob,
                                group.First(),
                                sequence
                            )
                        );

                    currentDeadline -= (maxProc + maxSetup);
                }

                fromJob = group.First();
            }
        }


        return result;
    }



    private double CalculateSetupTime(Machine m, Job job)
    {
        return _machineTimeCalculator
            .SetupTime(m.PreviousJob, job, job.SequenceNumber ?? 1);
    }

    private double GetReadyTimeFromPreviousSequence(Schedule schedule, Job job)
    {
        if (!schedule.Jobs.TryGetValue(job.ManufacturingOrderId, out var jobs))
            return 0;

        var prevSeq = jobs
            .Where(j => j.SequenceNumber < job.SequenceNumber)
            .Select(j => j.SequenceNumber)
            .DefaultIfEmpty(0)
            .Max();

        if (prevSeq == 0) return 0;

        return jobs
            .Where(j => j.SequenceNumber == prevSeq)
            .Max(j => j.EndTime);
    }

    private void PlaceJobOnMachine(
        Job job,
        Machine m,
        Schedule schedule,
        double moReadyTime)
    {
        var cap = job.EligibleMachines.First(x => x.Code == m.Code);

        double setup = CalculateSetupTime(m, job);
        double start = Math.Max(Math.Max(m.TotalTime, m.AvailableTime), moReadyTime);

        job.StartTime = start;
        job.SetupTime = setup;
        job.ProcessingTime = cap.ProcessingTime;
        job.EndTime = start + setup + cap.ProcessingTime;
        job.TotalTime = setup + cap.ProcessingTime;
        job.AssignedMachineCode = m.Code;

        m.PreviousJob = job;
        m.AvailableTime = job.EndTime;
        m.TotalTime = Math.Max(m.TotalTime, job.EndTime);

        schedule.Jobs[job.ManufacturingOrderId].Add(job);
    }

    // ================= UTIL =================

    private Machine CloneMachine(Machine m)
    {
        return new Machine
        {
            Id = m.Id,
            Code = m.Code,
            WorkCenterCode = m.WorkCenterCode,
            ClassId = m.ClassId,
            Properties = m.Properties,
            Capabilities = m.Capabilities,
            PreviousJob = null,
            AvailableTime = 0,
            TotalTime = 0
        };
    }

    private Solution FinalizeSchedule(Schedule schedule)
    {
        schedule.Makespan = schedule.Machines.Any()
            ? schedule.Machines.Max(m => m.TotalTime)
            : 0;

        return new Solution
        {
            BestSchedule = schedule,
            Makespan = _objectiveEvaluator.CalculateMakespan(schedule),
            TotalSetupTime = _objectiveEvaluator.CalculateTotalSetupTime(schedule),
            TotalLateness = _objectiveEvaluator.CalculateTotalLateness(schedule),
            BestObjective = _objectiveEvaluator.Evaluate(schedule),
            GeneratedAt = DateTime.UtcNow
        };
    }
}
