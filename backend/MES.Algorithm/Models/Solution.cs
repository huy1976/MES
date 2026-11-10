namespace MesMicroservice.Algorithm.Models
{
    public class Solution
    {
        public Schedule BestSchedule { get; set; }
        public double BestObjective { get; set; }
        public TimeSpan Runtime { get; set; }
        public int Iterations { get; set; }
        public double Makespan { get; set; }
        public double TotalSetupTime { get; set; }
        public double TotalLateness { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string ChosenHeuristic { get; set; } = string.Empty;
        public string Metaheuristic { get; set; } = string.Empty;

        /// <summary>
        /// Tạo bản sao đầy đủ của Solution.
        /// </summary>
        public Solution Clone()
        {
            return new Solution
            {
                BestSchedule = this.BestSchedule?.Clone(),
                BestObjective = this.BestObjective,
                Runtime = this.Runtime,
                Iterations = this.Iterations,
                Makespan = this.Makespan,
                TotalSetupTime = this.TotalSetupTime,
                TotalLateness = this.TotalLateness,
                GeneratedAt = this.GeneratedAt,
                ChosenHeuristic = this.ChosenHeuristic,
                Metaheuristic = this.Metaheuristic
            };
        }
    }
}
