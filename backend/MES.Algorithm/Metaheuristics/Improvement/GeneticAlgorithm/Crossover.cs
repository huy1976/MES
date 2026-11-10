global using GeneticSharp.Domain.Crossovers;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm
{
    /// <summary>
    /// Order crossover dành cho permutation chromosome (job ordering).
    /// Trả về hai offspring bằng cách giữ một segment từ parent1 và
    /// điền các gene còn lại theo thứ tự xuất hiện trong parent2.
    /// </summary>
    public class Crossover : ICrossover
    {
        public bool IsOrdered => true;

        public int ParentsNumber => 2;

        public int ChildrenNumber => 2;

        public int MinChromosomeLength => 2;

        /// <summary>
        /// Do crossover between two parents to create two offspring.
        /// </summary>
        /// <param name="parents"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IList<IChromosome> Cross(IList<IChromosome> parents)
        {
            if (parents == null) throw new ArgumentNullException(nameof(parents));
            if (parents.Count != ParentsNumber) throw new ArgumentException($"This crossover needs {ParentsNumber} parents.", nameof(parents));

            var p1 = (IChromosome)parents[0];
            var p2 = (IChromosome)parents[1];

            int length = p1.Length;
            var rnd = RandomizationProvider.Current;

            // choose two cut points
            int c1 = rnd.GetInt(0, length);
            int c2 = rnd.GetInt(0, length);
            if (c1 > c2) (c1, c2) = (c2, c1);

            var offspring1Genes = Enumerable.Repeat<object?>(null, length).ToArray();
            var offspring2Genes = Enumerable.Repeat<object?>(null, length).ToArray();

            // copy segment from parents
            for (int i = c1; i <= c2; i++)
            {
                offspring1Genes[i] = p1.GetGene(i).Value;
                offspring2Genes[i] = p2.GetGene(i).Value;
            }

            // helper to fill remaining preserving order from the other parent
            void FillFromParent(IChromosome srcParent, object?[] targetGenes)
            {
                // Thay vì dùng HashSet, ta dùng một mảng bool để đánh dấu cho nhanh
                bool[] used = new bool[length];
                // Giả sử JobId của bạn là số nguyên, ta cần logic để đánh dấu nhanh
                // Nếu JobId phức tạp, hãy dùng HashSet nhưng khởi tạo kích thước trước (capacity)
                var existing = new HashSet<object?>(length);
                for (int i = 0; i < length; i++) if (targetGenes[i] != null) existing.Add(targetGenes[i]);

                int pos = 0;
                for (int i = 0; i < length; i++)
                {
                    var g = srcParent.GetGene(i).Value;
                    if (existing.Contains(g)) continue;

                    while (pos < length && targetGenes[pos] != null) pos++;

                    if (pos < length)
                    {
                        targetGenes[pos] = g;
                        existing.Add(g);
                    }
                }
            }


            FillFromParent(p2, offspring1Genes);
            FillFromParent(p1, offspring2Genes);

            // build chromosomes
            var child1 = p1.CreateNew();
            var child2 = p1.CreateNew();

            for (int i = 0; i < length; i++)
            {
                child1.ReplaceGene(i, new Gene(offspring1Genes[i]));
                child2.ReplaceGene(i, new Gene(offspring2Genes[i]));
            }

            return new List<IChromosome> { child1, child2 };
        }
    }
}
