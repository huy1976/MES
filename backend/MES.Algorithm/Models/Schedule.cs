namespace MesMicroservice.Algorithm.Models
{
    public class Schedule
    {
        public List<Machine> Machines { get; set; } = new();
        public Dictionary<string, List<Job>> Jobs { get; set; } = new();
        public double Makespan { get; set; }
        public Schedule(List<Machine> machines, Dictionary<string, List<Job>> jobs)
        {
            Machines = machines;
            Jobs = jobs;
            Makespan = 0;
        }

        // Clone
        public Schedule Clone()
        {
            var clonedMachines = Machines.Select(m => m.Clone()).ToList();
            var clonedJobs = new Dictionary<string, List<Job>>();
            foreach (var kv in Jobs)
            {
                clonedJobs[kv.Key] = kv.Value.Select(j => j.Clone()).ToList();
            }
            return new Schedule(clonedMachines, clonedJobs)
            {
                Makespan = this.Makespan
            };
        }
    }

}
