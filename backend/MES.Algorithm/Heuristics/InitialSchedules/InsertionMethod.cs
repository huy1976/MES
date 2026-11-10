namespace MesMicroservice.Algorithm.Heuristics.InitialSchedules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InsertionMethod : IHeuristic
    {
        private readonly IEquipmentSelectorService _equipmentSelectorService;
        private readonly IMachineTimeCalculator _machineTimeCalculator;

        public InsertionMethod(
            IEquipmentSelectorService equipmentSelectorService,
            IMachineTimeCalculator machineTimeCalculator)
        {
            _equipmentSelectorService = equipmentSelectorService;
            _machineTimeCalculator = machineTimeCalculator;
        }

        public async Task<Solution> Run(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            var jobs = schedule.Jobs
                ?? throw new ArgumentException("Jobs cannot be null.", nameof(schedule));

            // Mỗi Job tự có danh sách máy phù hợp lấy từ Database.
            await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(jobs);
            _machineTimeCalculator.CalculateProcessingTimes(jobs);

            // Sinh thứ tự ban đầu mà không giả định số lượng công đoạn cố định.
            schedule.Jobs = GenerateInitialOrder(jobs);

            return new Solution
            {
                BestSchedule = schedule,
                ChosenHeuristic = EAlgorithmType.InsertionMethod.ToString()
            };
        }

        private Dictionary<string, List<Job>> GenerateInitialOrder(
            Dictionary<string, List<Job>> jobs)
        {
            var allJobs = jobs
                .SelectMany(kv => kv.Value)
                .Where(j => j != null)
                .OrderBy(j => j.ManufacturingOrderId)
                .ThenBy(j => j.SequenceNumber ?? 0)
                .ToList();

            if (allJobs.Count < 2)
                return jobs;

            // Step 1: chọn cặp khởi tạo tốt nhất.
            // Không còn stage 1 / stage 2 cố định.
            double minDistance = double.PositiveInfinity;
            Job? iStar = null;
            Job? jStar = null;

            foreach (var i in allJobs)
            {
                foreach (var j in allJobs)
                {
                    if (i.Id == j.Id)
                        continue;

                    // Nếu cùng một Manufacturing Order thì không được đảo thứ tự công đoạn.
                    if (!CanAppearBefore(i, j))
                        continue;

                    double pI = AvgProcTime(i);
                    double pJ = AvgProcTime(j);

                    if (double.IsPositiveInfinity(pI) ||
                        double.IsPositiveInfinity(pJ))
                    {
                        continue;
                    }

                    int stage = j.SequenceNumber ?? 1;
                    double setup = _machineTimeCalculator.SetupTime(i, j, stage);
                    double distance = setup + pI + pJ;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        iStar = i;
                        jStar = j;
                    }
                }
            }

            // Fallback an toàn nếu không tìm được cặp phù hợp.
            iStar ??= allJobs.First();
            jStar ??= allJobs.First(j => j.Id != iStar.Id);

            var sequence = new List<Job> { iStar, jStar };
            var unscheduled = allJobs.Except(sequence).ToList();

            // Step 2: lần lượt thử chèn các Job vào vị trí tốt nhất.
            while (unscheduled.Any())
            {
                Job? bestJob = null;
                int bestPosition = -1;
                double bestMakespan = double.PositiveInfinity;

                foreach (var job in unscheduled)
                {
                    var validPositions = Enumerable
                        .Range(0, sequence.Count + 1)
                        .Where(pos => IsValidInsertion(sequence, job, pos))
                        .ToList();

                    foreach (var pos in validPositions)
                    {
                        var temp = new List<Job>(sequence);
                        temp.Insert(pos, job);

                        double makespan = EstimateMakespan(temp);

                        if (makespan < bestMakespan)
                        {
                            bestMakespan = makespan;
                            bestJob = job;
                            bestPosition = pos;
                        }
                    }
                }

                if (bestJob != null && bestPosition >= 0)
                {
                    sequence.Insert(bestPosition, bestJob);
                    unscheduled.Remove(bestJob);
                }
                else
                {
                    // Fallback: chèn từng Job vào vị trí đầu tiên vẫn giữ đúng thứ tự công đoạn.
                    foreach (var job in unscheduled.ToList())
                    {
                        int fallbackPosition = Enumerable
                            .Range(0, sequence.Count + 1)
                            .FirstOrDefault(pos => IsValidInsertion(sequence, job, pos));

                        sequence.Insert(fallbackPosition, job);
                        unscheduled.Remove(job);
                    }
                }
            }

            // Cập nhật thứ tự toàn cục trong lịch.
            for (int i = 0; i < sequence.Count; i++)
            {
                sequence[i].IndexInSchedule = i;
            }

            // Trả lại đúng cấu trúc Dictionary<ManufacturingOrderId, List<Job>>.
            return sequence
                .GroupBy(j => j.ManufacturingOrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(j => j.IndexInSchedule).ToList()
                );
        }

        /// <summary>
        /// Ước lượng makespan bằng cách mô phỏng gán từng Job vào máy
        /// có thời điểm hoàn thành sớm nhất trong chính EligibleMachines của Job.
        /// Không phụ thuộc EC01 / EC02 / EC03 và không phụ thuộc số stage cố định.
        /// </summary>
        private double EstimateMakespan(List<Job> sequence)
        {
            // Thời điểm máy có thể nhận Job tiếp theo.
            var machineAvailableTime = new Dictionary<string, double>();

            // Job gần nhất đã chạy trên từng máy, dùng để tính setup.
            var previousJobOnMachine = new Dictionary<string, Job?>();

            // Thời điểm hoàn thành của từng công đoạn trong từng Manufacturing Order.
            var completionByStage = new Dictionary<(string MoId, int Stage), double>();

            double makespan = 0;

            foreach (var job in sequence)
            {
                if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                    return double.PositiveInfinity;

                int currentStage = job.SequenceNumber ?? 1;

                // Job chỉ được bắt đầu khi tất cả công đoạn trước của cùng MO đã hoàn thành.
                double previousStageReadyTime = completionByStage
                    .Where(x =>
                        x.Key.MoId == job.ManufacturingOrderId &&
                        x.Key.Stage < currentStage)
                    .Select(x => x.Value)
                    .DefaultIfEmpty(0)
                    .Max();

                Machine? bestMachine = null;
                double bestEndTime = double.PositiveInfinity;

                foreach (var machine in job.EligibleMachines)
                {
                    if (string.IsNullOrWhiteSpace(machine.Code))
                        continue;

                    double availableTime = machineAvailableTime.TryGetValue(machine.Code, out var savedAvailable)
                        ? savedAvailable
                        : machine.AvailableTime;

                    Job? previousJob = previousJobOnMachine.TryGetValue(machine.Code, out var savedPrevious)
                        ? savedPrevious
                        : machine.PreviousJob;

                    double setupTime = _machineTimeCalculator.SetupTime(
                        previousJob,
                        job,
                        currentStage
                    );

                    double startTime = Math.Max(availableTime, previousStageReadyTime);
                    double endTime = startTime + setupTime + machine.ProcessingTime;

                    if (endTime < bestEndTime)
                    {
                        bestEndTime = endTime;
                        bestMachine = machine;
                    }
                }

                if (bestMachine == null)
                    return double.PositiveInfinity;

                machineAvailableTime[bestMachine.Code] = bestEndTime;
                previousJobOnMachine[bestMachine.Code] = job;

                var stageKey = (job.ManufacturingOrderId, currentStage);
                if (completionByStage.TryGetValue(stageKey, out var oldCompletion))
                {
                    completionByStage[stageKey] = Math.Max(oldCompletion, bestEndTime);
                }
                else
                {
                    completionByStage[stageKey] = bestEndTime;
                }

                makespan = Math.Max(makespan, bestEndTime);
            }

            return makespan;
        }

        /// <summary>
        /// Trung bình thời gian gia công trên tất cả máy phù hợp của chính Job.
        /// Không còn lọc cứng theo EC01 / EC02 / EC03.
        /// </summary>
        private static double AvgProcTime(Job job)
        {
            if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                return double.PositiveInfinity;

            return job.EligibleMachines.Average(m => m.ProcessingTime);
        }

        /// <summary>
        /// Kiểm tra i có thể đứng trước j hay không.
        /// Chỉ có ràng buộc khi hai Job thuộc cùng Manufacturing Order.
        /// </summary>
        private static bool CanAppearBefore(Job i, Job j)
        {
            if (i.ManufacturingOrderId != j.ManufacturingOrderId)
                return true;

            return (i.SequenceNumber ?? 0) <= (j.SequenceNumber ?? 0);
        }

        /// <summary>
        /// Kiểm tra vị trí chèn có giữ đúng thứ tự công đoạn của Manufacturing Order không.
        /// Ví dụ 1 -> 2 -> 3 -> 4, không cho phép 3 đứng trước 2.
        /// </summary>
        private static bool IsValidInsertion(List<Job> sequence, Job job, int position)
        {
            int jobStage = job.SequenceNumber ?? 0;

            bool beforeIsValid = sequence
                .Take(position)
                .Where(x => x.ManufacturingOrderId == job.ManufacturingOrderId)
                .All(x => (x.SequenceNumber ?? 0) <= jobStage);

            if (!beforeIsValid)
                return false;

            bool afterIsValid = sequence
                .Skip(position)
                .Where(x => x.ManufacturingOrderId == job.ManufacturingOrderId)
                .All(x => (x.SequenceNumber ?? 0) >= jobStage);

            return afterIsValid;
        }
    }
}
