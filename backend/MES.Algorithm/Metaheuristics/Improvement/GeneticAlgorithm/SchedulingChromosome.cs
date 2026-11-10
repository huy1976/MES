global using GeneticSharp.Domain.Chromosomes;
global using GeneticSharp.Domain.Randomizations;
using MesMicroservice.Algorithm.Interfaces; // Đảm bảo bạn đã using interface này

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm
{
    public class SchedulingChromosome : ChromosomeBase
    {
        private readonly Schedule _baseSchedule;
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IAssignmentMethod _assignmentMethod; // Thêm biến lưu trữ Assignment
        public readonly List<Job> _jobs;
        private bool _isFirstCreation = true;

        // 1. Thêm IAssignmentMethod vào Constructor
        public SchedulingChromosome(
            List<int> jobIds,
            Schedule baseSchedule,
            IMachineTimeCalculator calc,
            IAssignmentMethod assignmentMethod)
            : base(jobIds.Count)
        {
            _baseSchedule = baseSchedule;
            _machineTimeCalculator = calc;
            _assignmentMethod = assignmentMethod; // Gán giá trị
            _jobs = _baseSchedule.Jobs.Values.SelectMany(j => j).ToList();

            for (int i = 0; i < jobIds.Count; i++)
                ReplaceGene(i, new Gene(jobIds[i]));
        }

        /// <summary>
        /// Do generate gene at index.
        /// </summary>
        public override Gene GenerateGene(int index)
        {
            return new Gene(_jobs[index].Id);
        }

        /// <summary>
        /// Do create a new chromosome.
        /// </summary>
        public override IChromosome CreateNew()
        {
            var jobIds = _baseSchedule.Jobs.Values.SelectMany(j => j).Select(j => j.Id).ToList();

            // 1. Nếu là lần tạo đầu tiên (Adam chromosome), GIỮ NGUYÊN THỨ TỰ CỦA INSERTION METHOD
            if (_isFirstCreation)
            {
                _isFirstCreation = false;
                // Không Shuffle
                return new SchedulingChromosome(jobIds, _baseSchedule, _machineTimeCalculator, _assignmentMethod);
            }

            // 2. Từ cá thể thứ 2 trở đi, mới xáo trộn để tạo sự đa dạng
            var rnd = RandomizationProvider.Current;
            rnd.Shuffle(jobIds);

            return new SchedulingChromosome(jobIds, _baseSchedule, _machineTimeCalculator, _assignmentMethod);
        }

        /// <summary>
        /// Decode chromosome to schedule using the existing Assignment Method.
        /// </summary>
        public Schedule DecodeToSchedule()
        {
            // Bước 1: Lấy danh sách JobId theo thứ tự mới từ Gene
            var geneJobIds = GetGenes().Select(g => (int)g.Value!).ToList();

            // Bước 2: Tạo bản sao sạch của Schedule
            var tempSchedule = DeepClone(_baseSchedule);

            // Gom tất cả các Job trong bản sao vào 1 Dictionary để tìm kiếm siêu tốc O(1)
            var allJobs = tempSchedule.Jobs.Values.SelectMany(j => j).ToDictionary(j => j.Id);

            // Bước 3: Cập nhật lại IndexInSchedule dựa trên thứ tự đột biến/lai ghép của GA
            for (int i = 0; i < geneJobIds.Count; i++)
            {
                if (allJobs.TryGetValue(geneJobIds[i], out var job))
                {
                    job.IndexInSchedule = i;
                }
            }

            //// Bước 4: Gọi giải thuật Assignment gánh vác phần việc khó nhất (gán máy, tính thời gian)
            //// Lấy thẳng .Result vì hàm Run của Assignment bọc Task.FromResult
            var solution = _assignmentMethod.Run(tempSchedule).Result;

            return solution.BestSchedule;
        }


        // === IMPORTANT: do NOT clone machine state ===
        private Schedule DeepClone(Schedule sch)
        {
            var clonedJobs = sch.Jobs.ToDictionary(
                kv => kv.Key,
                kv => kv.Value
                    .OrderBy(j => j.IndexInSchedule)
                    .Select(j => j.Clone())
                    .ToList()
            );

            return new Schedule(sch.Machines, clonedJobs);
        }
    }
}