using MesMicroservice.Algorithm.Heuristics.Assignment;
using System.Diagnostics;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using MesMicroservice.MachineLearning;
using static MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch.Move;
using System.Globalization;
using System.IO;
using MesMicroservice.Algorithm.Services;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch
{
    public class TabuSearchService : IMetaheuristic
    {
        private readonly IHeuristicFactory _initialHeuristic;
        private readonly IMachineTimeCalculator _machineTimeCalculator;
        private readonly IAssignmentMethod _assignmentMethod;
        private readonly ObjectiveEvaluator evaluator = new ObjectiveEvaluator();
        private readonly TabuAIPredictor _aiPredictor;

        public TabuSearchService(
    IHeuristicFactory initialHeuristic,
    IMachineTimeCalculator machineTimeCalculator,
    IAssignmentMethod assignmentMethod,
    TabuAIPredictor aiPredictor)
        {
            _initialHeuristic = initialHeuristic;
            _machineTimeCalculator = machineTimeCalculator;
            _assignmentMethod = assignmentMethod;
            _aiPredictor = aiPredictor;
        }


        public async Task<(int bestTenure, int bestNeighbors)> RefineAIParameters(
            Schedule initialSchedule, int aiTenure, int aiNeighbors)
        {
            var candidates = new List<(int T, int N)>
{
    (aiTenure, aiNeighbors),
    (5, 50),
    (8, 80),
    (11, 120),
    (15, 200),
    (20, 300)
};

            double bestObjective = double.MaxValue;
            int optimalT = 10;
            int optimalN = 20;

            foreach (var (t, n) in candidates)
            {
                var tempSettings = new TabuSearchSettings
                {
                    UseAI = false, // Chạy thử thông số cứng
                    TabuTenure = t,
                    NumNeighborsPerMoAndStage = n,
                    MaxIterations = 800 // Để MaxIter ngắn lại cho máy cày chạy nhanh hơn
                };

                // Gọi hàm chạy Tabu Search thực tế (Cần Clone schedule để không hỏng data)
                var result = await this.RunWithSettings(initialSchedule.Clone(), tempSettings, seed: 42);


                if (result.BestObjective < bestObjective)
                {
                    bestObjective = result.BestObjective;
                    optimalT = t;
                    optimalN = n;
                }
            }

            return (optimalT, optimalN);
        }

        public async Task<Solution> Run(Schedule schedule)
        {
            var settings = new TabuSearchSettings();
            return await RunWithSettings(schedule, settings);
        }

        private async Task<Schedule?> PathRelinking(Schedule source, Schedule target)

        {
            var sourceJobs = source.Jobs.Values.SelectMany(j => j).OrderBy(j => j.IndexInSchedule).ToList();
            var targetJobs = target.Jobs.Values.SelectMany(j => j).OrderBy(j => j.IndexInSchedule).ToList();
            var current = source.Clone();
            int pathLength = Math.Min(sourceJobs.Count, targetJobs.Count);
            for (int i = 0; i < pathLength; i++)
            {
                var currentJobs = current.Jobs.Values.SelectMany(j => j).OrderBy(j => j.IndexInSchedule).ToList();
                if (i >= currentJobs.Count || currentJobs[i].Id == targetJobs[i].Id)
                    continue;
                int fromIndex = currentJobs.FindIndex(j => j.Id == targetJobs[i].Id);

                if (fromIndex < 0) continue;

                var movingJob = currentJobs[fromIndex];
                currentJobs.RemoveAt(fromIndex);
                currentJobs.Insert(i, movingJob);

                for (int k = 0; k < currentJobs.Count; k++) currentJobs[k].IndexInSchedule = k;

                current.Jobs = currentJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());
                var decoded = await _assignmentMethod.Run(current);
                current = decoded.BestSchedule;
            }
            return current;
        }

        public async Task<Solution> RunWithSettings(Schedule schedule, TabuSearchSettings settings, int? seed = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var random = seed.HasValue
                ? new Random(seed.Value)
                : new Random(Guid.NewGuid().GetHashCode());

            var initial = await GetInitialSolution(schedule, _initialHeuristic);

            if (settings.UseAI)
            {
                var jobs = initial.BestSchedule.Jobs.Values
                    .SelectMany(x => x)
                    .ToList();

                var machines = initial.BestSchedule.Machines.ToList();
                var aiInput = AIFeatureExtractor.Extract(jobs, machines);
                double initialObjective = evaluator.Evaluate(initial.BestSchedule);
                double initialMakespan = evaluator.CalculateMakespan(initial.BestSchedule);

                aiInput.InitialMakespan = (float)initialMakespan;
                aiInput.InitialObjective = (float)initialObjective;

                var (aiTenure, aiNeighbors) = _aiPredictor.PredictBestParameters(aiInput);

                settings.TabuTenure = aiTenure;
                settings.NumNeighborsPerMoAndStage = aiNeighbors;
            }


            settings.MaxTenure = Math.Max(settings.MaxTenure, settings.TabuTenure);
            settings.MaxTenure = Math.Max(settings.MaxTenure, 80);

            settings.TabuTenure = Math.Clamp(
                settings.TabuTenure <= 0 ? 10 : settings.TabuTenure,
                settings.MinTenure,
                settings.MaxTenure);

            settings.NumNeighborsPerMoAndStage = Math.Clamp(
                settings.NumNeighborsPerMoAndStage <= 0 ? 20 : settings.NumNeighborsPerMoAndStage,
                20,
                500);

            settings.MaxIterations = Math.Max(1, settings.MaxIterations);
            settings.MaxIdleIterations = Math.Max(1, settings.MaxIdleIterations);
            var currentSchedule = initial.BestSchedule.Clone();
            var allJobs = currentSchedule.Jobs.Values.SelectMany(j => j).ToList();

            var setupCache = new SetupTransitionCache();
            setupCache.Build(allJobs);

            var generator = new NeighborhoodGenerator(_machineTimeCalculator, _assignmentMethod, setupCache);

            var bestSchedule = initial.BestSchedule.Clone();
            var moveFrequency = new Dictionary<int, int>();
            var eliteSolutions = new List<(Schedule schedule, double objective)>();
            const int MAX_ELITE_SOLUTIONS = 5;

            var bestObjective = evaluator.Evaluate(bestSchedule);

            if (settings.UseAI)
            {
                Console.WriteLine($"[AI] T={settings.TabuTenure} | N={settings.NumNeighborsPerMoAndStage}");
            }
            else
            {
                Console.WriteLine($"[Fixed] Tenure={settings.TabuTenure} | Neighbors={settings.NumNeighborsPerMoAndStage}");
            }

            int currentTenure = settings.TabuTenure;
            int mlSuggestedTenure = settings.TabuTenure;

            var tabuList = new TabuList(currentTenure);
            var visitedStates = new Dictionary<string, int>();
            double currentObjective = bestObjective;
            int iteration = 0;

            int idleIterations = 0;
            int MAX_IDLE_ITERATIONS = settings.MaxIdleIterations;

            int iterationsSinceLastTenureChange = 0;
            int escapeCounter = 0;
            const int ESCAPE_THRESHOLD = 15;

            visitedStates[GenerateScheduleHash(currentSchedule)] = 0;

            while (iteration++ < settings.MaxIterations)
            {
                tabuList.UpdateTenure(currentTenure);

                List<(Schedule schedule, Move move)> neighbors;

                if (random.NextDouble() < 0.7)
                {
                    neighbors = await generator.GenerateNeighbors_Insert(
                    currentSchedule,
                    settings.NumNeighborsPerMoAndStage,
    moveFrequency,
    random);
                }
                else
                {
                    neighbors = await generator.GenerateNeighbors_SwapIndex(
    currentSchedule,
    settings.NumNeighborsPerMoAndStage,
    random);
                }

                var (candidateSchedule, candidateMove, candidateObjective) = FindBestNeighbor(
                    neighbors,
                    tabuList,
                    bestObjective,
                    currentObjective,
                    moveFrequency,
                    visitedStates,
                    idleIterations);

                if (candidateSchedule == null)
                    break;

                currentSchedule = candidateSchedule;
                currentObjective = candidateObjective;

                if (settings.RTS)
                {
                    string currentHash = GenerateScheduleHash(currentSchedule);

                    if (visitedStates.ContainsKey(currentHash))
                    {
                        currentTenure = Math.Clamp(
    (int)(currentTenure * 1.2) + 1,
    settings.MinTenure,
    settings.MaxTenure);
                        iterationsSinceLastTenureChange = 0;
                        escapeCounter++;
                        visitedStates[currentHash] = iteration;
                    }
                    else
                    {
                        visitedStates[currentHash] = iteration;
                        iterationsSinceLastTenureChange++;
                        escapeCounter = 0;

                        if (iterationsSinceLastTenureChange > mlSuggestedTenure)
                        {
                            currentTenure = Math.Clamp(
    (int)(currentTenure * 0.9),
    settings.MinTenure,
    Math.Max(settings.MinTenure, mlSuggestedTenure));
                            iterationsSinceLastTenureChange = 0;
                        }
                    }

                    if (eliteSolutions.Count >= 2 && iteration % 500 == 0)
                    {
                        var eliteA = eliteSolutions[random.Next(eliteSolutions.Count)];
                        var eliteB = eliteSolutions[random.Next(eliteSolutions.Count)];

                        if (eliteA.schedule != eliteB.schedule)
                        {
                            var relinked = await PathRelinking(eliteA.schedule, eliteB.schedule);

                            if (relinked != null)
                            {
                                double relinkObj = evaluator.Evaluate(relinked);
                                if (relinkObj < bestObjective)
                                {
                                    bestObjective = relinkObj;
                                    bestSchedule = relinked.Clone();
                                    currentSchedule = relinked.Clone();
                                    currentObjective = relinkObj;
                                    Console.WriteLine($"[PathRelinking] Improved Objective -> {bestObjective:F2}");
                                }
                            }
                        }
                    }

                    if (escapeCounter >= ESCAPE_THRESHOLD)
                    {
                        var randomEscapeNeighbors = await generator.GenerateNeighbors_SwapIndex(
    currentSchedule,
    5,
    random);

                        if (randomEscapeNeighbors.Any())
                        {
                            currentSchedule = randomEscapeNeighbors[random.Next(randomEscapeNeighbors.Count)].Item1;
                            currentObjective = evaluator.Evaluate(currentSchedule);
                        }

                        tabuList.Clear();
                        currentTenure = mlSuggestedTenure;
                        escapeCounter = 0;
                        continue;
                    }
                }

                if (candidateMove != null)
                {
                    tabuList.Add(candidateMove);

                    if (!moveFrequency.ContainsKey(candidateMove.JobAId))
                        moveFrequency[candidateMove.JobAId] = 0;
                    moveFrequency[candidateMove.JobAId]++;

                    if (candidateMove.JobBId > 0)
                    {
                        if (!moveFrequency.ContainsKey(candidateMove.JobBId))
                            moveFrequency[candidateMove.JobBId] = 0;
                        moveFrequency[candidateMove.JobBId]++;
                    }
                }

                if (candidateObjective < bestObjective)
                {
                    bestObjective = candidateObjective;
                    bestSchedule = currentSchedule.Clone();
                    idleIterations = 0;

                    eliteSolutions.Add((currentSchedule.Clone(), candidateObjective));
                    eliteSolutions = eliteSolutions.OrderBy(x => x.objective).Take(MAX_ELITE_SOLUTIONS).ToList();
                }
                else
                {
                    idleIterations++;
                }

                if (idleIterations >= MAX_IDLE_ITERATIONS)
                {
                    Console.WriteLine($"[Early Stopping] Thuật toán tự ngắt tại vòng {iteration} do đã {MAX_IDLE_ITERATIONS} vòng liên tiếp không cải thiện được Objective.");
                    break;
                }
            }

            stopwatch.Stop();
            return new Solution
            {
                BestSchedule = bestSchedule,
                BestObjective = bestObjective,
                Runtime = stopwatch.Elapsed,
                Iterations = iteration - 1,
                Makespan = evaluator.CalculateMakespan(bestSchedule),
                TotalSetupTime = evaluator.CalculateTotalSetupTime(bestSchedule),
                TotalLateness = evaluator.CalculateTotalLateness(bestSchedule),
                Metaheuristic = EAlgorithmType.TabuSearch.ToString(),
                GeneratedAt = DateTime.UtcNow
            };
        }

        private async Task<Solution> GetInitialSolution(Schedule schedule, IHeuristicFactory factory)
        {
            var ins = factory.Create(HeuristicType.InsertionMethod);
            var insSol = await ins.Run(schedule.Clone());
            var insDecoded = await _assignmentMethod.Run(insSol.BestSchedule);
            insDecoded.ChosenHeuristic = HeuristicType.InsertionMethod.ToString();
            return insDecoded;
        }

        private (Schedule? schedule, Move? move, double objective) FindBestNeighbor(
            IEnumerable<(Schedule schedule, Move move)> neighbors,
            TabuList tabuList, double bestObjective, double currentObjective,
            Dictionary<int, int> moveFrequency, Dictionary<string, int> visitedStates, int idleIterations)
        {
            Schedule? bestSchedule = null;
            Move? bestMove = null;
            double bestAdjustedObj = double.MaxValue;
            double bestRealObj = double.MaxValue;

            foreach (var (schedule, move) in neighbors)
            {
                double obj = evaluator.Evaluate(schedule);
                bool isTabu = tabuList.IsTabu(move);

                if (isTabu && obj >= bestObjective) continue;

                double diversificationBonus = 0;
                

                double frequencyPenalty = 0;
                if (move != null)
                {
                    if (moveFrequency.ContainsKey(move.JobAId))
                        frequencyPenalty += currentObjective * 0.0001 * moveFrequency[move.JobAId];

                    if (move.JobBId > 0 && moveFrequency.ContainsKey(move.JobBId))
                        frequencyPenalty += currentObjective * 0.0001 * moveFrequency[move.JobBId];
                }

                double repetitionPenalty = 0;
                string hash = GenerateScheduleHash(schedule);
                if (visitedStates.ContainsKey(hash))
                    repetitionPenalty = Math.Max(1000, currentObjective * 0.02);


                double adjustedObj = obj - diversificationBonus + frequencyPenalty + repetitionPenalty;

                if (adjustedObj < bestAdjustedObj)
                {
                    bestAdjustedObj = adjustedObj;
                    bestRealObj = obj;
                    bestSchedule = schedule;
                    bestMove = move;
                }
            }
            return (bestSchedule, bestMove, bestRealObj);
        }

        private string GenerateScheduleHash(Schedule schedule)
        {
            if (schedule.Jobs == null || !schedule.Jobs.Any()) return string.Empty;

            var allJobsFlat = schedule.Jobs.Values
                .SelectMany(x => x)
                .OrderBy(j => j.IndexInSchedule)
                .ThenBy(j => j.Id);

            return string.Join("|", allJobsFlat.Select(j =>
    $"{j.Id}:{j.IndexInSchedule}:{j.AssignedMachineCode}"));
        }

    }
}