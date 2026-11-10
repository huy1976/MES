using MesMicroservice.Domain.AggregateModels.ScheduleAggregate;

namespace MesMicroservice.Application.DTOs.WorkOrderDTOs
{
    public class WorkOrderViewDTO
    {
        public int ManufacturingOrderId { get; set; }
        public string ManufacturingOrderCode { get; set; } = string.Empty;
        public int Id { get; set; }
        public string WorkOrderId { get; private set; }
        public string SegmentId { get; private set; }
        public string? SegmentName { get; set; } = string.Empty;
        public int? SequenceNumber { get; set; }
        public string MaterialDefinitionId { get; set; }
        public decimal ProductionTarget { get; private set; }
        public string Unit { get; private set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string ProductionParamId { get; set; }
        public string QualityParamId { get; set; }
        public string Status { get; set; }
        public List<EquipmentWorkOrderDTO> Equipments { get; set; } = new List<EquipmentWorkOrderDTO>();
        public List<PersonWorkOrderDTO> Persons { get; set; } = new List<PersonWorkOrderDTO>();
        public List<MaterialWorkOrderDTO> Materials { get; set; } = new List<MaterialWorkOrderDTO>();
        public List<string> BOMs { get; set; } = new List<string>();
        public DateTime? ActuallyStartTime { get; set; }
        public DateTime? ActuallyEndTime { get; set; }
        public double Duration { get; set; }
        public double SetupTime { get; set; }
        public double RunTime { get; set; }
        public DateTime? PlannedStartTime => Start;
        public DateTime? PlannedEndTime => End;
        public List<GanttBarDTO> GanttBars { get; set; } = new();
        public WorkOrderViewDTO(int manufacturingOrderId, string manufacturingOrderCode,int id, string workOrderId, string segmentId, int? sequenceNumber, string materialDefinitionId, decimal productionTarget, string unit, DateTime? start, DateTime? end, string productionParamId, string qualityParamId, string status)
        {
            ManufacturingOrderId = manufacturingOrderId;
            ManufacturingOrderCode = manufacturingOrderCode;
            Id = id;
            WorkOrderId = workOrderId;
            SegmentId = segmentId;
            SequenceNumber = sequenceNumber;
            MaterialDefinitionId = materialDefinitionId;
            ProductionTarget = productionTarget;
            Unit = unit;
            Start = start;
            End = end;
            ProductionParamId = productionParamId;
            QualityParamId = qualityParamId;
            Status = status;
        }
        public class GanttBarDTO
        {
            public string Type { get; set; } = string.Empty; // Plan / Actual
            public string Lane { get; set; } = string.Empty; // VD: KT06 - Kế hoạch
            public string EquipmentId { get; set; } = string.Empty;
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool IsRunning { get; set; }
            public bool IsCompleted { get; set; }
            public bool IsDelayed { get; set; }
            public string Color { get; set; } = string.Empty;
        }
        public void UpdateSegmentName(string? segmentName)
        {
            SegmentName = segmentName;
        }

        public void AddBOMItem(string bomItem)
        {
            BOMs.Add(bomItem);
        }

    }

    public class EquipmentWorkOrderDTO
    {
        public string Id { get; private set; }
        public string EquipmentId { get; private set; }
        public string ClassName { get; private set; }

        public EquipmentWorkOrderDTO(string id, string equipmentId, string className)
        {
            Id = id;
            EquipmentId = equipmentId;
            ClassName = className;
        }
    }

    public class MaterialWorkOrderDTO
    {
        public string Id { get; private set; }
        public string MaterialDefinitionId { get; private set; }
        public string ClassName { get; private set; }
        public Value Quantity { get; private set; }
        public string ValueUnitOfMeasure { get; private set; }
        public double RequireRawComponentQuantityPerDay { get; set; }
        public double ActualRawComponentQuantityPerDay { get; set; }
        public double ActualRawComponentQuantity { get; set; }

        public MaterialWorkOrderDTO(string id, string materialDefinitionId, string className, Value quantity, string valueUnitOfMeasure)
        {
            Id = id;
            MaterialDefinitionId = materialDefinitionId;
            ClassName = className;
            Quantity = quantity;
            ValueUnitOfMeasure = valueUnitOfMeasure;
        }
    }

    public class PersonWorkOrderDTO
    {
        public string Id { get; private set; }
        public string PersonId { get; private set; }
        public string ClassName { get; private set; }
        public string Shift { get; private set; } = string.Empty;

        public PersonWorkOrderDTO(string id, string personId, string className, string shift)
        {
            Id = id;
            PersonId = personId;
            ClassName = className;
            Shift = shift;
        }
    }



}
