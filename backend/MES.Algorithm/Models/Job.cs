global using MesMicroservice.Domain.Enums;

namespace MesMicroservice.Algorithm.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Job
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? SequenceNumber { get; set; } // Thứ tự công đoạn
        public int IndexInSchedule { get; set; } // Chỉ số trong lịch trình

        // Không còn SegmentType ở tầng Algorithm.
        // Algorithm chỉ cần SequenceNumber + EligibleMachines để lập lịch.

        public bool IsLateInSchedule { get; set; } = false;
        public double ProcessingTime { get; set; }
        public double? ConfiguredProcessingTimeHours { get; set; }
        public double SetupTime { get; set; }
        public double TeardownTime { get; set; }
        public double TotalTime { get; set; }
        public double DueDate { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }

        public string? AssignedMachineCode { get; set; }

        public string ManufacturingOrderId { get; set; } = string.Empty;
        public string MaterialDefinitionId { get; set; } = string.Empty;
        public decimal Quatity { get; set; }
        public double Price { get; set; }
        public double Diameter { get; set; }
        public double InputMaterialDiameter { get; set; }
        public double Density { get; set; }
        public string Type { get; set; } = string.Empty;
        public string EquipmentType = string.Empty;
        public double SpeedRetirement { get; set; }
        public int RetireLayers { get; set; }
        public List<string> MaterialSpecInfors { get; set; } = new();
        public List<Machine> EligibleMachines { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
        public int Priority { get; set; } = 1;
        public bool IsUrgent => Priority > 1;

        public Job Clone()
        {
            var clonedJob = new Job
            {
                Id = this.Id,
                Name = this.Name,
                SequenceNumber = this.SequenceNumber,
                IndexInSchedule = this.IndexInSchedule,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                SetupTime = this.SetupTime,
                TeardownTime = this.TeardownTime,
                ProcessingTime = this.ProcessingTime,
                ConfiguredProcessingTimeHours = this.ConfiguredProcessingTimeHours,
                TotalTime = this.TotalTime,
                DueDate = this.DueDate,
                AssignedMachineCode = this.AssignedMachineCode,
                ManufacturingOrderId = this.ManufacturingOrderId,
                MaterialDefinitionId = this.MaterialDefinitionId,
                Diameter = this.Diameter,
                Density = this.Density,
                Quatity = this.Quatity,
                Type = this.Type,
                RetireLayers = this.RetireLayers,
                MaterialSpecInfors = new List<string>(this.MaterialSpecInfors),
                Employees = this.Employees?.Select(e => e.Clone()).ToList() ?? new List<Employee>(),
                InputMaterialDiameter = this.InputMaterialDiameter,
                EquipmentType = this.EquipmentType,
                SpeedRetirement = this.SpeedRetirement,
                Price = this.Price,
                IsLateInSchedule = this.IsLateInSchedule,
                Priority = this.Priority
            };

            if (this.EligibleMachines != null && this.EligibleMachines.Any())
            {
                clonedJob.EligibleMachines = this.EligibleMachines
                    .Select(m => m.Clone())
                    .ToList();
            }

            return clonedJob;
        }
    }
}
