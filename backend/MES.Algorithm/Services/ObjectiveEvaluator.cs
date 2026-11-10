namespace MesMicroservice.Algorithm.Services
{
    using System;
    using System.Linq;

    public class ObjectiveEvaluator
    {
        /// <summary>
        /// Tính giá trị mục tiêu tổng hợp của lịch (Schedule).
        /// Áp dụng cấu trúc trọng số (w1, w2, w3, w4) và tính phạt trễ hạn theo NGÀY.
        /// </summary>
        public double Evaluate(Schedule schedule)
        {
            if (schedule == null || schedule.Machines == null || schedule.Machines.Count == 0)
                return double.MaxValue;

            // 1. Cập nhật lại thời gian của từng máy
            UpdateMachineTimes(schedule);

            // 2. Tính các thành phần cơ bản
            double makespan = CalculateMakespan(schedule);
            double totalSetupTime = CalculateTotalSetupTime(schedule);
            double penalty = new CheckCalendarService().CalculatePenalty(schedule);

            // 3. Tính tiền phạt trễ hạn (0.1% giá trị đơn hàng / NGÀY)
            double totalWeightedTardiness = 0.0;
            double urgentDueDatePenalty = 0.0;
            const double urgentPenaltyPerHour = 100000.0;
            // ĐÃ CHỈNH SỬA: 0.1% tương đương với 0.001
            double percent = 0.001;
            var jobs = schedule.Jobs.SelectMany(x => x.Value).ToList();

            foreach (var moGroup in jobs.GroupBy(j => j.ManufacturingOrderId))
            {
                var moJobs = moGroup.ToList();
                double completion = moJobs.Max(j => j.EndTime);
                double due = moJobs.Max(j => j.DueDate);
                double latenessInHours = Math.Max(0, completion - due);
                int priority = moJobs.Max(j => j.Priority);

                if (latenessInHours > 0)
                {
                    double latenessInDays = latenessInHours / 24.0;

                    double moValue = moJobs.Sum(j => j.Price * (double)j.Quatity);
                    double dynamicWeight = percent * moValue;

                    totalWeightedTardiness += latenessInDays * dynamicWeight;

                    if (priority > 0)
                    {
                        urgentDueDatePenalty += latenessInHours * priority * urgentPenaltyPerHour;
                    }
                }
            }

            // 4. KHAI BÁO CÁC TRỌNG SỐ (Weights)
            double w1 = 1.0;  // Trọng số Makespan

            double salary = 12000000;
            double w2 = Math.Round(salary / 208.0, 2); // Trọng số Setup (Tiền lương 1 giờ ~ 57,692)

            double w3 = 1.0;  // Trọng số Lateness

            double w4 = 50.0; // Trọng số Calendar Penalty

            double setupFragmentationPenalty = 0;
            foreach (var machine in schedule.Machines)
            {
                var machineJobs = schedule.Jobs.Values.SelectMany(j => j)
                    .Where(j => j.AssignedMachineCode == machine.Code)
                    .OrderBy(j => j.StartTime).ToList();

                for (int i = 1; i < machineJobs.Count; i++)
                {
                    bool materialChanged = machineJobs[i].MaterialDefinitionId != machineJobs[i - 1].MaterialDefinitionId;
                    bool diameterChanged = Math.Abs(machineJobs[i].Diameter - machineJobs[i - 1].Diameter) > 0.01;

                    if (materialChanged || diameterChanged)
                    {
                        setupFragmentationPenalty += 3; // Phạt 3 điểm mỗi lần đổi khuôn/vật liệu
                    }
                }
            }

            // 5. Hàm mục tiêu tổng hợp
            double objective = (w1 * makespan)
                 + (w2 * totalSetupTime)
                 + (w3 * totalWeightedTardiness)
                 + (w4 * penalty)
                 + setupFragmentationPenalty
                 + urgentDueDatePenalty;

            schedule.Makespan = makespan;
            return Math.Round(objective, 2);
        }

        /// <summary>
        /// Cập nhật lại TotalTime của từng máy theo EndTime các job đang gán cho máy đó.
        /// </summary>
        private void UpdateMachineTimes(Schedule schedule)
        {
            if (schedule?.Machines == null || schedule.Jobs == null)
                return;

            foreach (var machine in schedule.Machines)
            {
                var jobsOnMachine = schedule.Jobs.Values
                    .SelectMany(x => x)
                    .Where(j => j.AssignedMachineCode == machine.Code)
                    .ToList();

                machine.TotalTime = jobsOnMachine.Any() ? jobsOnMachine.Max(j => j.EndTime) : 0;
            }
        }

        /// <summary>
        /// Tính Makespan = thời gian hoàn thành lớn nhất trong các máy.
        /// </summary>
        public double CalculateMakespan(Schedule schedule)
        {
            if (schedule?.Jobs != null)
            {
                double maxJobEndTime = schedule.Jobs.Values
                    .SelectMany(j => j)
                    .Select(j => j.EndTime)
                    .DefaultIfEmpty(0)
                    .Max();

                if (maxJobEndTime > 0)
                    return maxJobEndTime;
            }

            return (schedule?.Machines ?? Enumerable.Empty<Machine>())
                .Select(m => m.TotalTime)
                .DefaultIfEmpty(0)
                .Max();
        }

        /// <summary>
        /// Tính tổng thời gian setup toàn bộ job.
        /// </summary>
        public double CalculateTotalSetupTime(Schedule schedule)
        {
            if (schedule?.Jobs == null)
                return 0;

            return schedule.Jobs
                .SelectMany(j => j.Value)
                .Sum(job => job.SetupTime);
        }

        /// <summary>
        /// Tính tổng độ trễ (lateness) của tất cả job có DueDate > 0.
        /// </summary>
        public double CalculateTotalLateness(Schedule schedule)
        {
            if (schedule?.Jobs == null)
                return 0;

            double totalLateness = 0;
            var allJobs = schedule.Jobs.Values.SelectMany(j => j);

            foreach (var moGroup in allJobs.GroupBy(j => j.ManufacturingOrderId))
            {
                var moJobs = moGroup.ToList();
                double completion = moJobs.Max(j => j.EndTime);
                double due = moJobs.Max(j => j.DueDate);

                double lateness = Math.Max(0, completion - due);
                totalLateness += lateness;
            }

            return totalLateness;
        }
    }
}