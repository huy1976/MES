namespace MesMicroservice.Algorithm.Services
{
    /// <summary>
    /// Csv logger extended: logs iteration,makespan,totalLateness,objective,elapsed, timestamp
    /// </summary>
    internal class CsvLoggerExtended : IDisposable
    {
        private readonly string _filePath;
        private readonly StreamWriter _writer;

        public CsvLoggerExtended(string algorithmName)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "output");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string fileName = $"{algorithmName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            _filePath = Path.Combine(baseDir, fileName);

            _writer = new StreamWriter(_filePath, false, System.Text.Encoding.UTF8);
            _writer.WriteLine("Iteration,Makespan,BestMakespan,CurrentTotalLateness,BestTotalLateness,CurrentTotalSetup,BestTotalSetup,CurrentPenalty,BestPenalty,CurrentObjective, BestObjective,ElapsedSeconds,Timestamp");
            _writer.Flush();
        }

        public void Log(
            int iteration,
            double makespan,
            double bestMakespan,
            double currentTotalLateness,
            double bestTotalLateness,
            double currentTotalSetup,
            double bestTotalSetup,
            double currentPenalty,
            double bestPenalty,
            double currentObjective,
            double bestObjective,
            double elapsedSeconds)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            // Chuyển elapsedSeconds sang định dạng hh:mm:ss.ffffff
            TimeSpan runtime = TimeSpan.FromSeconds(elapsedSeconds);
            string runtimeStr = runtime.ToString(@"hh\:mm\:ss\.ffffff", culture);

            string line = string.Join(",",
                iteration.ToString(culture),
                makespan.ToString("F6", culture),
                bestMakespan.ToString("F6", culture),
                currentTotalLateness.ToString("F6", culture),
                bestTotalLateness.ToString("F6", culture),
                currentTotalSetup.ToString("F6", culture),
                bestTotalSetup.ToString("F6", culture),
                currentPenalty.ToString("F6", culture),
                bestPenalty.ToString("F6", culture),
                currentObjective.ToString("F6", culture),
                bestObjective.ToString("F6", culture),
                runtimeStr,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", culture)
            );

            _writer.WriteLine(line);
            _writer.Flush();
        }



        public void Dispose()
        {
            _writer.Flush();
            _writer.Close();
            _writer.Dispose();
        }

        public string GetFilePath() => _filePath;
    }
}
