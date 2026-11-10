namespace MesMicroservice.Application.DTOs.WorkOrderDTOs
{
    public class WorkOrderProgressViewDTO
    {
        public int ManufacturingOrderId { get; set; }
        public string ManufacturingOrderCode { get; set; } = string.Empty;
        public int Id { get; set; }
        public string WorkOrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SegmentId { get; set; } = string.Empty; 
        public string? SegmentName { get; set; } = string.Empty;
        public int? SequenceNumber { get; set; }
        public string MaterialDefinitionId { get; set; } = string.Empty;
        public decimal ProductionTarget { get; set; }
        public double ProductionTargetByDay { get; set; }
        public double ProducedQuantity { get; set; }
        public double DeltaQuantity { get; set; }
        public double ProgressPercentage { get; set; }
        public double ErrorQuantity { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public DateTime? PlanedStart { get; set; }
        public DateTime? PlanedEnd { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string ProductionParamId { get; set; } = string.Empty;
        public string QualityParamId { get; set; }  = string.Empty;
        public List<EquipmentWorkOrderDTO> Equipments { get; set; } = new List<EquipmentWorkOrderDTO>();
        public List<PersonWorkOrderDTO> Persons { get; set; } = new List<PersonWorkOrderDTO>();
        public List<MaterialWorkOrderDTO> Materials { get; set; } = new List<MaterialWorkOrderDTO>();


    }
}
