global using GeneticSharp.Domain.Fitnesses;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm
{
    public class SchedulingFitness : IFitness
    {
        private readonly ObjectiveEvaluator _evaluator = new();
        private readonly CheckCalendarService _checkService = new();

        /// <summary>
        /// Evaluate the fitness of a chromosome.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double Evaluate(IChromosome chromosome)
        {
            if (chromosome is not SchedulingChromosome sc)
                throw new ArgumentException("Chromosome must be SchedulingChromosome", nameof(chromosome));

            var c = (SchedulingChromosome)chromosome;
            var jobIds = c.GetGenes().Select(g => (int)g.Value).ToList();
            if (ViolatesSequenceInMO(jobIds, c._jobs))
            {
                return 0;
            }

            NormalizeIndex(c._jobs);

            var schedule = sc.DecodeToSchedule();

            // Tính hàm mục tiêu
            double obj = _evaluator.Evaluate(schedule);
            double totalCost = obj;

            // Chuyển minimize -> maximize
            return 1.0 / (1.0 + totalCost);
        }
        // Hàm kiểm tra ràng buộc quy trình tối ưu
        private bool ViolatesSequenceInMO(List<int> currentOrderIds, List<Job> baseJobs)
        {
            var allJobsDict = baseJobs.ToDictionary(j => j.Id);
            var lastSequencePerMO = new Dictionary<string, int>();

            foreach (var jobId in currentOrderIds)
            {
                if (!allJobsDict.TryGetValue(jobId, out var job)) continue;

                string moId = job.ManufacturingOrderId;
                int currentSeq = job.SequenceNumber ?? 0;

                if (lastSequencePerMO.TryGetValue(moId, out int lastSeq))
                {
                    // Nếu Job hiện tại có Sequence thấp hơn Job trước đó của cùng MO -> Vi phạm
                    if (currentSeq < lastSeq) return true;
                }

                lastSequencePerMO[moId] = currentSeq;
            }
            return false;
        }
        private static void NormalizeIndex(List<Job> jobs)
        {
            jobs.OrderBy(j => j.IndexInSchedule)
                .ThenBy(j => j.Id)
                .Select((j, i) => (j, i))
                .ToList()
                .ForEach(x => x.j.IndexInSchedule = x.i);
        }

    }
}
