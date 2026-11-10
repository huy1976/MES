namespace MesMicroservice.Algorithm.Services
{
    public class SetupTransitionCache
    {
        private readonly Dictionary<(int fromId, int toId), double>
            _cache = new();

        public void Build(List<Job> jobs)
        {
            foreach (var from in jobs)
            {
                foreach (var to in jobs)
                {
                    _cache[(from.Id, to.Id)] =
                        CalculateSetup(from, to);
                }
            }
        }

        public double Get(int fromId, int toId)
        {
            return _cache.TryGetValue(
                (fromId, toId),
                out var value)
                ? value
                : 0;
        }

        private double CalculateSetup(Job prev, Job current)
        {
            bool sameMaterialSpec =
                new HashSet<string>(prev.MaterialSpecInfors)
                    .SetEquals(current.MaterialSpecInfors);

            bool sameDiameter =
                Math.Abs(prev.Diameter - current.Diameter) < 0.01;

            if (sameMaterialSpec && sameDiameter)
                return 0;

            return 1;
        }
    }
}