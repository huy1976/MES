namespace MesMicroservice.Algorithm.Models
{
    public class Employee
    {
        public string PersonId { get; set; } 
        public string Name { get; set; }
        public string MachineId { get; set; }
        public bool shift1 { get; set; }
        public bool shift2 { get; set; }
        public bool shift3 { get; set; }

        // Clone
        public Employee Clone()
        {
            return new Employee
            {
                PersonId = this.PersonId,
                Name = this.Name,
                MachineId = this.MachineId,
                shift1 = this.shift1,
                shift2 = this.shift2,
                shift3 = this.shift3
            };
        }
    }
}
