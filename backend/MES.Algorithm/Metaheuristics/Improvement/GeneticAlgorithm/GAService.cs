using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using MesMicroservice.Algorithm.Interfaces;
using System.Diagnostics;
using System.Globalization;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm
{
    public class GAService : IMetaheuristic
    {
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IHeuristicFactory _initialHeuristic;
        private readonly IAssignmentMethod _assignmentMethod;
        private readonly ObjectiveEvaluator evaluator = new ObjectiveEvaluator();

        public GAService(IMachineTimeCalculator machineTimeCalculator, IHeuristicFactory initialHeuristic, IAssignmentMethod assignmentMethod)
        {
            _machineTimeCalculator = machineTimeCalculator;
            _initialHeuristic = initialHeuristic;
            _assignmentMethod = assignmentMethod;
        }


        /// <summary>
        /// Run Genetic Algorithm to improve the given schedule.
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public async Task<Solution> Run(Schedule schedule)
        {
            var _config = new GAConfig();
            var fitness = new SchedulingFitness();
            var heuristics = new[] { HeuristicType.GuptaDarrow1, HeuristicType.InsertionMethod };

            // ===== INITIAL SOLUTION =====
            var initSol = await GetInitialSolution(schedule, _initialHeuristic);

            var currentSchedule = initSol.BestSchedule.Clone();
            var bestSchedule = initSol.BestSchedule.Clone();
            var bestObjective = evaluator.Evaluate(bestSchedule);

            //// === Tính các chỉ tiêu ===
            //double makespan = evaluator.CalculateMakespan(bestSchedule);
            //double lateness = evaluator.CalculateTotalLateness(bestSchedule);
            //double setup = evaluator.CalculateTotalSetupTime(bestSchedule);

            var calendar = new CheckCalendarService();
            // 1. Lấy danh sách ID của các Job theo thứ tự hiện tại từ lịch trình ban đầu (Insertion Method)
            var jobIds = (currentSchedule ?? schedule).Jobs.Values
                            .SelectMany(j => j)
                            .OrderBy(j => j.IndexInSchedule) // Đảm bảo giữ đúng thứ tự của Insertion Method
                            .Select(j => j.Id)
                            .ToList();
            // === Khởi tạo quần thể ===
            var chromosome = new SchedulingChromosome(jobIds, currentSchedule ?? schedule, _machineTimeCalculator, _assignmentMethod);
            var selection = new GeneticSharp.Domain.Selections.EliteSelection();
            var crossover = new Crossover();
            var mutation = new Mutation();

            var population = new Population(_config.PopulationSize, _config.PopulationSize, chromosome);

            var ga = new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                MutationProbability = (float)_config.MutationRate,
                CrossoverProbability = (float)_config.CrossoverRate,
                Termination = new GenerationNumberTermination(_config.MaxGeneration),
                TaskExecutor = new GeneticSharp.Infrastructure.Framework.Threading.ParallelTaskExecutor()

            };

            if (_config.EnableElitism)
                ga.Reinsertion = new GeneticSharp.Domain.Reinsertions.ElitistReinsertion();

            var stopwatch = Stopwatch.StartNew();
            using var logger = new CsvLoggerExtended($"{EAlgorithmType.GeneticAlgorithm.ToString()}_{initSol.ChosenHeuristic}");
            int iteration = 0;

            ga.GenerationRan += async (sender, args) =>
            {
                iteration++;

                // === Best hiện tại ===
                var bestChromosome = (SchedulingChromosome)ga.BestChromosome.Clone();
                var currentSchedule = bestChromosome.DecodeToSchedule();

                // === Tổng chi phí đánh giá ===
                double currentObjective = evaluator.Evaluate(currentSchedule);
                double penalty = calendar.CalculatePenalty(currentSchedule);
                double totalCost = currentObjective + penalty;

                // === Cập nhật global best (theo tổng chi phí) ===
                if (totalCost < bestObjective)
                {
                    bestObjective = totalCost;
                    bestSchedule = currentSchedule.Clone();
                }

                LogState(
                    logger,
                    iteration,
                    currentSchedule,
                    bestSchedule,
                    stopwatch,
                    calendar.CalculatePenalty(currentSchedule),
                    calendar.CalculatePenalty(bestSchedule));
            };

            await Task.Run(() => ga.Start());
            stopwatch.Stop();

            // === Trả kết quả tốt nhất tìm được ===
            var finalSchedule = bestSchedule ?? schedule;
            var bestSolution = new Solution
            {
                BestSchedule = finalSchedule,
                BestObjective = bestObjective,
                Makespan = evaluator.CalculateMakespan(finalSchedule),
                TotalSetupTime = evaluator.CalculateTotalSetupTime(finalSchedule),
                TotalLateness = evaluator.CalculateTotalLateness(finalSchedule),
                Iterations = iteration,
                Runtime = stopwatch.Elapsed,
                ChosenHeuristic = initSol.ChosenHeuristic,
                Metaheuristic = EAlgorithmType.GeneticAlgorithm.ToString()

            };

            //var checkSchedule = new CheckCalendarService();
            //if (!checkSchedule.ChecksSchedule(bestSolution.BestSchedule))
            //    throw new Exception("TabuSearch: Schedule check failed!");

            return bestSolution;
        }


        private async Task<Solution> GetInitialSolution(
    Schedule schedule,
    IHeuristicFactory factory)
        {
            var ins = factory.Create(HeuristicType.InsertionMethod);
            var insSol = await ins.Run(schedule.Clone());
            var insDecoded = await _assignmentMethod.Run(insSol.BestSchedule);
            var insScore = evaluator.Evaluate(insDecoded.BestSchedule);
            insDecoded.ChosenHeuristic = HeuristicType.InsertionMethod.ToString();
            return insDecoded;
        }

        private void LogState(
    CsvLoggerExtended logger,
    int iteration,
    Schedule current,
    Schedule best,
    Stopwatch stopwatch,
    double penalty,
    double bestPenalty)
        {
            logger.Log(
                iteration,
                evaluator.CalculateMakespan(current),
                evaluator.CalculateMakespan(best),
                evaluator.CalculateTotalLateness(current),
                evaluator.CalculateTotalLateness(best),
                evaluator.CalculateTotalSetupTime(current),
                evaluator.CalculateTotalSetupTime(best),
                penalty * 0.5,
                bestPenalty * 0.5,
                evaluator.Evaluate(current),
                evaluator.Evaluate(best),
                stopwatch.Elapsed.TotalSeconds);
        }
    }
}
