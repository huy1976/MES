namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm
{
    public class GAConfig
    {
        public int PopulationSize { get; set; } = 200;     // tăng quần thể → đa dạng nghiệm
        public int ChromosomeLength { get; set; } = 500;   // tuỳ số job, giữ nguyên hoặc tự động tính
        public int MaxGeneration { get; set; } = 2000;     // gấp đôi để tiến hóa lâu hơn

        public double CrossoverRate { get; set; } = 0.9;   // lai ghép mạnh → khám phá nhiều
        public double MutationRate { get; set; } = 0.2;    // đột biến cao hơn → tránh local optimum
        public bool EnableElitism { get; set; } = true;    // giữ cá thể tốt nhất
    }

}
