namespace MesMicroservice.Algorithm.Services
{
    public class CheckCalendarService
    {
        public bool ChecksSchedule(Schedule schedule)
        {
            if (HasOverlappingJobs(schedule))
                return false;

            if (ViolatesPrecedenceConstraints(schedule))
                return false;

            foreach (var jobList in schedule.Jobs.Values)
            {
                foreach (var j in jobList)
                {
                    Console.WriteLine($"After: {j.Id}: Start={j.StartTime}, End={j.EndTime}, Due={j.DueDate}");
                }
            }
            if (ViolatesDueDates(schedule))
                return false;

            return true;

        }

        public double CalculatePenalty(Schedule schedule)
        {
            double penalty = 0;

            if (HasOverlappingJobs(schedule))
                penalty += 3; // tương đương 3h trễ => 300 điểm

            if (ViolatesPrecedenceConstraints(schedule))
                penalty += 3; // tương đương 3h trễ => 300 điểm

            return penalty;
        }


        // Kiểm tra xem có job nào bị trùng lịch không
        private bool HasOverlappingJobs(Schedule schedule)
        {
            foreach (var machine in schedule.Machines)
            {
                var jobsOnMachine = schedule.Jobs.Values
                    .SelectMany(x => x)
                    .Where(j => j.AssignedMachineCode == machine.Code)
                    .OrderBy(j => j.StartTime)
                    .ToList();

                for (int i = 1; i < jobsOnMachine.Count; i++)
                {
                    if (jobsOnMachine[i].StartTime < jobsOnMachine[i - 1].EndTime)
                    {
                        return true; // Tìm thấy job trùng lịch
                    }
                }
            }
            return false; // Không có job trùng lịch
        }

        // Kiểm tra xem có vi phạm ràng buộc thứ tự công đoạn không
        private bool ViolatesPrecedenceConstraints(Schedule schedule)
        {
            // Kiểm tra thứ tự công đoạn trong từng MO
            foreach (var jobs in schedule.Jobs.Values)
            {
                // Lấy danh sách SequenceNumber có thứ tự tăng dần
                var sequenceGroups = jobs
                    .GroupBy(j => j.SequenceNumber)
                    .OrderBy(g => g.Key)
                    .ToList();

                for (int i = 1; i < sequenceGroups.Count; i++)
                {
                    // Thời gian kết thúc trễ nhất của các job trước
                    var prevMaxEndTime = sequenceGroups[i - 1].Max(j => j.EndTime);

                    // Thời gian bắt đầu sớm nhất của job hiện tại
                    var currMinStartTime = sequenceGroups[i].Min(j => j.StartTime);

                    if (currMinStartTime < prevMaxEndTime)
                    {
                        return true; // Vi phạm thứ tự công đoạn
                    }
                }
            }

            return false; // Không vi phạm
        }


        private bool ViolatesDueDates(Schedule schedule)
        {
            foreach (var jobList in schedule.Jobs.Values)
            {
                foreach (var job in jobList)
                {
                    if (job.EndTime > job.DueDate)
                        return true; // Job trễ hạn
                }
            }
            return false;
        }

        // --- Tính mức phạt trễ hạn (có trọng số theo thời gian trễ) ---
        private double CalculateDueDatePenalty(Schedule schedule)
        {
            double penalty = 0;
            const double weight = 0.1; // hệ số phạt theo thời gian trễ

            foreach (var jobList in schedule.Jobs.Values)
            {
                foreach (var job in jobList)
                {
                    if (job.EndTime > job.DueDate)
                    {
                        var lateness = (job.EndTime - job.DueDate);
                        penalty += weight * lateness; // phạt theo số phút trễ
                    }
                }
            }
            return penalty;
        }


    }
}
