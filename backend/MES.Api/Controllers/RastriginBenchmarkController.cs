using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Text;

// Paste this controller into your API project, for example:
// MesMicroservice.Api/Controllers/RastriginBenchmarkController.cs
//
// Endpoint:
// POST /api/RastriginBenchmark/run
//
// Recommended body:
// {
//   "step": 0.01,
//   "runs": 30,
//   "tabuIterations": 2000,
//   "tabuTenure": 50,
//   "gaPopulation": 80,
//   "gaGenerations": 200,
//   "gaMutationRate": 0.12,
//   "maxDegreeOfParallelism": 0
// }

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RastriginBenchmarkController : ControllerBase
    {
        private const double SearchRange = 5.12;
        private const double KnownOptimumValue = 1000.0;

        [HttpPost("run")]
        public IActionResult Run([FromBody] RastriginBenchmarkRequest request)
        {
            request ??= new RastriginBenchmarkRequest();

            if (request.Step <= 0 || request.Step > 1)
                return BadRequest("Step phai nam trong khoang (0, 1].");

            if (request.Runs <= 0)
                request.Runs = 30;

            int minIndex = (int)Math.Round(-SearchRange / request.Step);
            int maxIndex = (int)Math.Round(SearchRange / request.Step);

            bool gridContainsZero = minIndex <= 0 && maxIndex >= 0;
            if (!gridContainsZero)
                return BadRequest("Grid phai chua diem 0 de BruteForce tim dung nghiem toi uu da biet.");

            int maxDegreeOfParallelism = request.MaxDegreeOfParallelism <= 0
                ? Math.Max(1, Environment.ProcessorCount - 1)
                : request.MaxDegreeOfParallelism;

            var bruteForce = RunBruteForce(minIndex, maxIndex, request.Step, maxDegreeOfParallelism);

            var rows = new List<RastriginBenchmarkRow>();
            for (int run = 1; run <= request.Runs; run++)
            {
                int seed = request.Seed + run;

                var tabu = RunTabuSearch(
                    minIndex,
                    maxIndex,
                    request.Step,
                    request.TabuIterations,
                    request.TabuTenure,
                    seed);

                var ga = RunGeneticAlgorithm(
                    minIndex,
                    maxIndex,
                    request.Step,
                    request.GaPopulation,
                    request.GaGenerations,
                    request.GaMutationRate,
                    seed);

                rows.Add(new RastriginBenchmarkRow
                {
                    Run = run,
                    Step = request.Step,
                    BruteForceX = bruteForce.X,
                    BruteForceY = bruteForce.Y,
                    BruteForceObjective = bruteForce.Objective,
                    BruteForceRuntimeMs = bruteForce.RuntimeMs,
                    TabuX = tabu.X,
                    TabuY = tabu.Y,
                    TabuObjective = tabu.Objective,
                    TabuGapPercent = GapPercent(tabu.Objective),
                    TabuRuntimeMs = tabu.RuntimeMs,
                    GaX = ga.X,
                    GaY = ga.Y,
                    GaObjective = ga.Objective,
                    GaGapPercent = GapPercent(ga.Objective),
                    GaRuntimeMs = ga.RuntimeMs,
                    Winner = GetWinner(bruteForce.Objective, tabu.Objective, ga.Objective)
                });
            }

            string filePath = Path.Combine(
                AppContext.BaseDirectory,
                $"Rastrigin_Benchmark_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            System.IO.File.WriteAllText(filePath, BuildCsv(rows), Encoding.UTF8);

            return Ok(new
            {
                Message = "Hoan tat benchmark Rastrigin.",
                Source = "Rastrigin has known global minimum f(0,0)=0. This benchmark uses shifted objective F=1000+f, so optimum is 1000.",
                SearchDomain = "x,y in [-5.12, 5.12]",
                KnownOptimum = new { X = 0, Y = 0, Objective = KnownOptimumValue },
                BruteForceFound = new
                {
                    bruteForce.X,
                    bruteForce.Y,
                    Objective = Math.Round(bruteForce.Objective, 8),
                    GapPercent = Math.Round(GapPercent(bruteForce.Objective), 8),
                    bruteForce.RuntimeMs
                },
                SavedAt = filePath,
                Summary = new
                {
                    Runs = rows.Count,
                    AverageTabuGapPercent = Math.Round(rows.Average(x => x.TabuGapPercent), 6),
                    BestTabuGapPercent = Math.Round(rows.Min(x => x.TabuGapPercent), 6),
                    AverageGaGapPercent = Math.Round(rows.Average(x => x.GaGapPercent), 6),
                    BestGaGapPercent = Math.Round(rows.Min(x => x.GaGapPercent), 6),
                    TabuExactRuns = rows.Count(x => Math.Abs(x.TabuObjective - KnownOptimumValue) < 1e-9),
                    GaExactRuns = rows.Count(x => Math.Abs(x.GaObjective - KnownOptimumValue) < 1e-9)
                },
                Details = rows
            });
        }

        private static SearchResult RunBruteForce(
            int minIndex,
            int maxIndex,
            double step,
            int maxDegreeOfParallelism)
        {
            var sw = Stopwatch.StartNew();
            object bestLock = new object();
            var globalBest = new SearchResult { Objective = double.MaxValue };

            Parallel.For(
                minIndex,
                maxIndex + 1,
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                () => new SearchResult { Objective = double.MaxValue },
                (ix, _, localBest) =>
                {
                    for (int iy = minIndex; iy <= maxIndex; iy++)
                    {
                        double x = ToValue(ix, step);
                        double y = ToValue(iy, step);
                        double objective = Objective(x, y);

                        if (objective < localBest.Objective)
                        {
                            localBest = new SearchResult
                            {
                                X = x,
                                Y = y,
                                Objective = objective
                            };
                        }
                    }

                    return localBest;
                },
                localBest =>
                {
                    lock (bestLock)
                    {
                        if (localBest.Objective < globalBest.Objective)
                            globalBest = localBest;
                    }
                });

            sw.Stop();
            globalBest.RuntimeMs = sw.Elapsed.TotalMilliseconds;
            return globalBest;
        }

        private static SearchResult RunTabuSearch(
            int minIndex,
            int maxIndex,
            double step,
            int iterations,
            int tabuTenure,
            int seed)
        {
            iterations = iterations <= 0 ? 2000 : iterations;
            tabuTenure = tabuTenure <= 0 ? 50 : tabuTenure;

            var random = new Random(seed);
            var sw = Stopwatch.StartNew();

            var current = new PointIndex(
                random.Next(minIndex, maxIndex + 1),
                random.Next(minIndex, maxIndex + 1));

            var best = Evaluate(current, step);
            var tabu = new Dictionary<PointIndex, int>();
            var directions = new[]
            {
                (-1, -1), (-1, 0), (-1, 1),
                (0, -1),           (0, 1),
                (1, -1),  (1, 0),  (1, 1)
            };

            for (int iteration = 1; iteration <= iterations; iteration++)
            {
                SearchResult? bestCandidate = null;
                PointIndex bestCandidatePoint = current;

                foreach (var (dx, dy) in directions)
                {
                    var next = new PointIndex(
                        Math.Clamp(current.X + dx, minIndex, maxIndex),
                        Math.Clamp(current.Y + dy, minIndex, maxIndex));

                    bool isTabu = tabu.TryGetValue(next, out int expireAt) && expireAt > iteration;
                    var candidate = Evaluate(next, step);

                    bool aspiration = candidate.Objective < best.Objective;
                    if (isTabu && !aspiration)
                        continue;

                    if (bestCandidate == null || candidate.Objective < bestCandidate.Objective)
                    {
                        bestCandidate = candidate;
                        bestCandidatePoint = next;
                    }
                }

                if (bestCandidate == null)
                {
                    current = new PointIndex(
                        random.Next(minIndex, maxIndex + 1),
                        random.Next(minIndex, maxIndex + 1));
                    continue;
                }

                tabu[current] = iteration + tabuTenure;
                current = bestCandidatePoint;

                if (bestCandidate.Objective < best.Objective)
                    best = bestCandidate;
            }

            sw.Stop();
            best.RuntimeMs = sw.Elapsed.TotalMilliseconds;
            return best;
        }

        private static SearchResult RunGeneticAlgorithm(
            int minIndex,
            int maxIndex,
            double step,
            int populationSize,
            int generations,
            double mutationRate,
            int seed)
        {
            populationSize = populationSize <= 0 ? 80 : populationSize;
            generations = generations <= 0 ? 200 : generations;
            mutationRate = mutationRate <= 0 ? 0.12 : mutationRate;

            var random = new Random(seed);
            var sw = Stopwatch.StartNew();

            var population = Enumerable.Range(0, populationSize)
                .Select(_ => new PointIndex(
                    random.Next(minIndex, maxIndex + 1),
                    random.Next(minIndex, maxIndex + 1)))
                .ToList();

            for (int generation = 0; generation < generations; generation++)
            {
                population = population
                    .OrderBy(p => Evaluate(p, step).Objective)
                    .ToList();

                var nextPopulation = new List<PointIndex>
                {
                    population[0],
                    population[1]
                };

                while (nextPopulation.Count < populationSize)
                {
                    var parentA = Tournament(population, step, random);
                    var parentB = Tournament(population, step, random);

                    var child = new PointIndex(
                        random.NextDouble() < 0.5 ? parentA.X : parentB.X,
                        random.NextDouble() < 0.5 ? parentA.Y : parentB.Y);

                    if (random.NextDouble() < mutationRate)
                    {
                        child = new PointIndex(
                            Math.Clamp(child.X + random.Next(-10, 11), minIndex, maxIndex),
                            Math.Clamp(child.Y + random.Next(-10, 11), minIndex, maxIndex));
                    }

                    nextPopulation.Add(child);
                }

                population = nextPopulation;
            }

            var best = population
                .Select(p => Evaluate(p, step))
                .OrderBy(x => x.Objective)
                .First();

            sw.Stop();
            best.RuntimeMs = sw.Elapsed.TotalMilliseconds;
            return best;
        }

        private static PointIndex Tournament(List<PointIndex> population, double step, Random random)
        {
            const int tournamentSize = 3;
            return Enumerable.Range(0, tournamentSize)
                .Select(_ => population[random.Next(population.Count)])
                .OrderBy(p => Evaluate(p, step).Objective)
                .First();
        }

        private static SearchResult Evaluate(PointIndex point, double step)
        {
            double x = ToValue(point.X, step);
            double y = ToValue(point.Y, step);
            return new SearchResult
            {
                X = x,
                Y = y,
                Objective = Objective(x, y)
            };
        }

        private static double Objective(double x, double y)
        {
            double rastrigin =
                20.0
                + x * x - 10.0 * Math.Cos(2.0 * Math.PI * x)
                + y * y - 10.0 * Math.Cos(2.0 * Math.PI * y);

            return KnownOptimumValue + rastrigin;
        }

        private static double ToValue(int index, double step)
        {
            return Math.Round(index * step, 10);
        }

        private static double GapPercent(double objective)
        {
            return ((objective - KnownOptimumValue) / KnownOptimumValue) * 100.0;
        }

        private static string GetWinner(double bruteForce, double tabu, double ga)
        {
            double best = new[] { bruteForce, tabu, ga }.Min();
            if (Math.Abs(bruteForce - best) < 1e-9)
                return "BruteForce";
            if (Math.Abs(tabu - best) < 1e-9)
                return "Tabu";
            return "GA";
        }

        private static string BuildCsv(List<RastriginBenchmarkRow> rows)
        {
            var culture = CultureInfo.InvariantCulture;
            var sb = new StringBuilder();
            sb.AppendLine(
                "Run,Step," +
                "BruteForceX,BruteForceY,BruteForceObjective,BruteForceRuntimeMs," +
                "TabuX,TabuY,TabuObjective,TabuGapPercent,TabuRuntimeMs," +
                "GaX,GaY,GaObjective,GaGapPercent,GaRuntimeMs,Winner");

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",",
                    row.Run,
                    row.Step.ToString(culture),
                    row.BruteForceX.ToString(culture),
                    row.BruteForceY.ToString(culture),
                    row.BruteForceObjective.ToString(culture),
                    row.BruteForceRuntimeMs.ToString(culture),
                    row.TabuX.ToString(culture),
                    row.TabuY.ToString(culture),
                    row.TabuObjective.ToString(culture),
                    row.TabuGapPercent.ToString(culture),
                    row.TabuRuntimeMs.ToString(culture),
                    row.GaX.ToString(culture),
                    row.GaY.ToString(culture),
                    row.GaObjective.ToString(culture),
                    row.GaGapPercent.ToString(culture),
                    row.GaRuntimeMs.ToString(culture),
                    row.Winner));
            }

            return sb.ToString();
        }

        private readonly record struct PointIndex(int X, int Y);

        private class SearchResult
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Objective { get; set; }
            public double RuntimeMs { get; set; }
        }

        public class RastriginBenchmarkRequest
        {
            public double Step { get; set; } = 0.01;
            public int Runs { get; set; } = 30;
            public int Seed { get; set; } = 2026;
            public int TabuIterations { get; set; } = 2000;
            public int TabuTenure { get; set; } = 50;
            public int GaPopulation { get; set; } = 80;
            public int GaGenerations { get; set; } = 200;
            public double GaMutationRate { get; set; } = 0.12;
            public int MaxDegreeOfParallelism { get; set; } = 0;
        }

        public class RastriginBenchmarkRow
        {
            public int Run { get; set; }
            public double Step { get; set; }
            public double BruteForceX { get; set; }
            public double BruteForceY { get; set; }
            public double BruteForceObjective { get; set; }
            public double BruteForceRuntimeMs { get; set; }
            public double TabuX { get; set; }
            public double TabuY { get; set; }
            public double TabuObjective { get; set; }
            public double TabuGapPercent { get; set; }
            public double TabuRuntimeMs { get; set; }
            public double GaX { get; set; }
            public double GaY { get; set; }
            public double GaObjective { get; set; }
            public double GaGapPercent { get; set; }
            public double GaRuntimeMs { get; set; }
            public string Winner { get; set; } = "";
        }
    }
}
