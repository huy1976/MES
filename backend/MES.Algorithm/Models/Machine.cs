namespace MesMicroservice.Algorithm.Models
{
    public class Machine
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string WorkCenterCode { get; set; } = "";
        public string ClassId { get; set; } = "";
        public double ProcessingTime { get; set; } = 0;
        public double TotalTime { get; set; } = 0;
        public double AvailableTime { get; set; } = 0;
        public Job? PreviousJob { get; set; } = null;
        public MachineProperties Properties { get; set; } = new();
        public List<MachineCapabilityProperty> Capabilities { get; set; } = new();
        

        public Machine Clone()
        {
            return new Machine
            {
                Id = this.Id,
                Code = this.Code,
                WorkCenterCode = this.WorkCenterCode,
                ClassId = this.ClassId,
                TotalTime = this.TotalTime,
                ProcessingTime = this.ProcessingTime,
                Properties = this.Properties.Clone(),
                Capabilities = this.Capabilities.Select(c => new MachineCapabilityProperty(c.Name, c.Description, c.Value, c.Unit)).ToList(),
                AvailableTime = this.AvailableTime,
                PreviousJob = this.PreviousJob?.Clone() // Note: shallow copy; ensure Job is immutable or handle accordingly
            };
        }


    }
    public class MachineCapabilityProperty
    {
        public string Name { get; set; } 
        public string Description { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }

        public MachineCapabilityProperty(string name, string description, double value, string unit)
        {
            Name = name;
            Description = description;
            Value = value;
            Unit = unit;
        }
    }

    public class MachineProperties
    {
        public double Efficiency { get; set; } = 0.78;
        public double Speed { get; set; } = 0;
        public double Priority { get; set; } = 0;
        public double SetupTimeMin { get; set; } = 0;
        public double SetupTimeMax { get; set; } = 0;
        public double DryTime { get; set; } = 0;
        public double AnnealDwellTime { get; set; } = 0;
        public double OvenLength { get; set; } = 0;
        public int Heads { get; set; } = 1;
        public int Lines { get; set; } = 1;
        public int Layers { get; set; } = 1;
        public string DryType { get; set; } = "";
        public double MinDiameter { get; set; } = 0;
        public double MaxDiameter { get; set; } = 0;
        public double InputMin { get; set; } = 0;
        public double InputMax { get; set; } = 0;

        public MachineProperties Clone()
        {
            return (MachineProperties)this.MemberwiseClone();
        }
    }


}
