global using GeneticSharp.Domain.Mutations;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm
{
    /// <summary>
    /// Swap mutation: hoán đổi 2 gene bất kỳ trong chromosome (dùng cho permutation).
    /// </summary>
    public class Mutation : IMutation
    {
        public bool IsOrdered => true; // permutation-based

        /// <summary>
        /// Do mutation on the given chromosome with the given probability.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <param name="probability"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Mutate(IChromosome chromosome, float probability)
        {
            if (chromosome == null) throw new ArgumentNullException(nameof(chromosome));

            var rnd = RandomizationProvider.Current;
            if (rnd.GetDouble() >= probability) return;

            var length = chromosome.Length;
            if (length < 2) return;

            // Chọn 2 điểm cắt
            int i = rnd.GetInt(0, length);
            int j = rnd.GetInt(0, length);
            if (i > j) (i, j) = (j, i);

            // THỰC HIỆN INVERSION (ĐẢO NGƯỢC ĐOẠN)
            // Cách này hiệu quả hơn rất nhiều cho Scheduling so với Swap
            var genes = chromosome.GetGenes();
            while (i < j)
            {
                var tempI = genes[i];
                var tempJ = genes[j];

                chromosome.ReplaceGene(i, tempJ);
                chromosome.ReplaceGene(j, tempI);

                i++;
                j--;
            }
        }
    }
}
