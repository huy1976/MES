global using MesMicroservice.Algorithm.Heuristics.Assignment;
global using MesMicroservice.Algorithm.Models.Mapping;
global using MesMicroservice.Algorithm.Services;

namespace MesMicroservice.Algorithm.Heuristics.InitialSchedules
{
    public class GuptaDarrow1 : IHeuristic
    {
        private readonly IEquipmentSelectorService _equipmentSelectorService;
        private readonly IMachineTimeCalculator _machineTimeCalculator;

        public GuptaDarrow1(
            IEquipmentSelectorService equipmentSelectorService,
            IMachineTimeCalculator machineTimeCalculator)
        {
            _equipmentSelectorService = equipmentSelectorService;
            _machineTimeCalculator = machineTimeCalculator;
        }

        #region === GD1 value functions (JobChain level) ===

        private double AValue(JobChain c, JobChain? left, JobChain? right)
        {
            var first = c.Jobs.First();
            var last = c.Jobs.Last();

            // Không còn hard-code stage 1.
            // Nếu sau này SetupTime sử dụng stage, ta truyền đúng SequenceNumber của Job.
            double setupFirst = left == null
                ? 0
                : _machineTimeCalculator.SetupTime(
                    left.Jobs.First(),
                    first,
                    first.SequenceNumber.GetValueOrDefault());

            double procFirst = AvgProcTime(first);

            // Không còn hard-code stage 2.
            double setupLast = 0;
            if (right != null)
            {
                setupLast = _machineTimeCalculator.SetupTime(
                    last,
                    right.Jobs.Last(),
                    last.SequenceNumber.GetValueOrDefault());
            }

            return setupFirst + procFirst - setupLast;
        }

        private double BValue(JobChain c)
        {
            var last = c.Jobs.Last();

            // [ĐÃ BỎ HARD-CODE]
            // Không còn yêu cầu Job cuối phải là ESegmentType.Enameling.
            // Job cuối là công đoạn nào sẽ do dữ liệu quy trình quyết định.
            return AvgProcTime(last);
        }

        private double C1(JobChain c, JobChain? left)
        {
            var first = c.Jobs.First();

            double setup = left == null
                ? 0
                : _machineTimeCalculator.SetupTime(
                    left.Jobs.First(),
                    first,
                    first.SequenceNumber.GetValueOrDefault());

            return setup + AvgProcTime(first);
        }

        private double C2(JobChain c, JobChain? right)
        {
            var last = c.Jobs.Last();

            double setup = right == null
                ? 0
                : _machineTimeCalculator.SetupTime(
                    last,
                    right.Jobs.Last(),
                    last.SequenceNumber.GetValueOrDefault());

            return AvgProcTime(last) + setup;
        }

        #endregion

        public async Task<Solution> Run(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            var jobsByKey = schedule.Jobs
                ?? throw new ArgumentException("Jobs cannot be null.", nameof(schedule));

            // 1) Chuẩn bị dữ liệu
            await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(jobsByKey);
            _machineTimeCalculator.CalculateProcessingTimes(jobsByKey);

            // 2) Gom Job -> JobChain (theo Manufacturing Order)
            var chains = BuildJobChains(jobsByKey);

            if (chains.Count <= 1)
            {
                ApplyIndexInSchedule(chains);

                schedule.Jobs = RebuildJobDictionary(jobsByKey);

                return new Solution
                {
                    BestSchedule = schedule,
                    ChosenHeuristic = EAlgorithmType.GuptaDarrow1.ToString()
                };
            }

            // 3) Chạy Gupta-Darrow 1 trên JobChain
            var orderedChains = GenerateInitialChainOrder(chains);

            // 4) Trải JobChain -> Job + IndexInSchedule
            ApplyIndexInSchedule(orderedChains);

            // 5) Trả lại cấu trúc Dictionary<string, List<Job>>
            schedule.Jobs = RebuildJobDictionary(jobsByKey);

            // 6) Return
            return new Solution
            {
                BestSchedule = schedule,
                ChosenHeuristic = EAlgorithmType.GuptaDarrow1.ToString()
            };
        }

        #region === Core GD1 on JobChain ===

        private List<JobChain> GenerateInitialChainOrder(List<JobChain> chains)
        {
            var unscheduled = new List<JobChain>(chains);
            var S = new List<JobChain>();

            JobChain? left = null;
            JobChain? right = null;

            while (unscheduled.Any())
            {
                JobChain iStar = unscheduled[0];
                JobChain jStar = unscheduled[0];

                double minA = double.PositiveInfinity;
                double minB = double.PositiveInfinity;

                foreach (var c in unscheduled)
                {
                    double a = AValue(c, left, right);
                    if (a < minA)
                    {
                        minA = a;
                        iStar = c;
                    }

                    double b = BValue(c);
                    if (b < minB)
                    {
                        minB = b;
                        jStar = c;
                    }
                }

                if (minA < minB)
                {
                    S.Insert(0, iStar);
                    left = iStar;
                    unscheduled.Remove(iStar);
                }
                else if (minA > minB)
                {
                    S.Add(jStar);
                    right = jStar;
                    unscheduled.Remove(jStar);
                }
                else
                {
                    if (Math.Min(C1(iStar, left), C2(jStar, right))
                        <= Math.Min(C1(jStar, left), C2(iStar, right)))
                    {
                        S.Insert(0, iStar);
                        left = iStar;
                        unscheduled.Remove(iStar);
                    }
                    else
                    {
                        S.Add(jStar);
                        right = jStar;
                        unscheduled.Remove(jStar);
                    }
                }
            }

            return S;
        }

        #endregion

        #region === Helpers ===

        private List<JobChain> BuildJobChains(Dictionary<string, List<Job>> jobs)
        {
            return jobs
                .SelectMany(kv => kv.Value)
                .Where(j => j != null)
                .GroupBy(j => j.ManufacturingOrderId)
                .Select(g => new JobChain
                {
                    ManufacturingOrderId = g.Key,
                    Jobs = g.OrderBy(j => j.SequenceNumber).ToList()
                })
                .Where(c => c.Jobs.Count > 0)
                .ToList();
        }

        private void ApplyIndexInSchedule(List<JobChain> chains)
        {
            int index = 0;

            foreach (var chain in chains)
            {
                // Luôn giữ đúng thứ tự công đoạn trong cùng một Manufacturing Order.
                foreach (var job in chain.Jobs.OrderBy(j => j.SequenceNumber))
                {
                    job.IndexInSchedule = index++;
                }
            }
        }

        private Dictionary<string, List<Job>> RebuildJobDictionary(
            Dictionary<string, List<Job>> original)
        {
            var allJobs = original
                .SelectMany(kv => kv.Value)
                .OrderBy(j => j.IndexInSchedule)
                .ToList();

            return allJobs
                .GroupBy(j => j.ManufacturingOrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(j => j.IndexInSchedule).ToList()
                );
        }

        private double AvgProcTime(Job job)
        {
            if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                return double.MaxValue;

            // [ĐÃ BỎ HARD-CODE]
            // Không còn:
            // stage == 1 -> EC01 / EC03
            // stage == 2 -> EC02 / EC03
            //
            // EligibleMachines đã được EquipmentSelectorService xác định từ dữ liệu.
            // GuptaDarrow1 chỉ cần sử dụng danh sách máy phù hợp của chính Job.
            return job.EligibleMachines.Average(m => m.ProcessingTime);
        }

        internal class JobChain
        {
            public string ManufacturingOrderId { get; set; } = string.Empty;
            public List<Job> Jobs { get; set; } = new();
        }

        #endregion
    }
}
