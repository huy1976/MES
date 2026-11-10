namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch
{
    public class TabuSearchSettings
    {
        public int MaxIterations { get; set; } = 2000;

        public int MaxIdleIterations { get; set; } = 2000;

        public int TabuTenure { get; set; }

        public int NumNeighborsPerMoAndStage { get; set; }

        public bool UseAI { get; set; } = true;

        public bool RTS { get; set; } = true;

        public int MinTenure { get; set; } = 5;

        public int MaxTenure { get; set; } = 80;

        public int RepetitionThreshold { get; set; } = 3;
    }
}