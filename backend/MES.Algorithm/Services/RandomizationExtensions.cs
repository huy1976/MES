namespace MesMicroservice.Algorithm.Services
{
    public static class RandomizationExtensions
    {
        public static void Shuffle<T>(this IRandomization rnd, IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.GetInt(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
