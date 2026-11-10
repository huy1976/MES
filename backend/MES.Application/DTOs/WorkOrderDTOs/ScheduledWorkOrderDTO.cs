namespace MesMicroservice.Application.DTOs.WorkOrderDTOs
{
    public class ScheduledWorkOrderDTO
    {
        public string WorkOrderId { get; private set; }
        public string SegmentId { get; private set; }
        public string WorkCenter { get; private set; }
        public int? SequenceNumber { get; set; }
        public string MaterialDefinition { get; set; }
        public decimal ProductionTarget { get; private set; }
        public string Unit { get; private set; }
        public DateTime DueDate { get; private set; }
        public double Duration { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public DateTime? ActuallyStartTime { get; private set; }
        public DateTime? ActuallyEndTime { get; private set; }
        public EStatus WorkOrderStatus { get; private set; }
        public string ProductionParameterId { get; set; }
        public string QualityStandardId { get; set; }
        public double SetupTime { get; set; }
        public double RunTime { get; set; }
        public bool? IsLateInSchedule => EndTime?.CompareTo(DueDate) > 0;
        public double? Lateness { get; set; } = 0;
        public string ManufacturingOrderId { get; private set; }
        public string EquipmentId { get; private set; }
        public string PersonShift12 { get; private set; }
        public string PersonShift3 { get; private set; }
        public List<string> BOMs { get; set; } = new List<string>();

        public ScheduledWorkOrderDTO(string workOrderId, string segmentId, string workCenter, int? sequenceNumber, string materialDefinition, decimal productionTarget, string unit, DateTime dueDate, double duration, DateTime? startTime, DateTime? endTime, DateTime? actuallyStartTime, DateTime? actuallyEndTime, EStatus workOrderStatus, string qualityStandardId, double setupTime, double runTime, string manufacturingOrderId, string equipmentId, string personShift12, string personShift3, string productionParameterId)
        {
            WorkOrderId = workOrderId;
            SegmentId = segmentId;
            WorkCenter = workCenter;
            SequenceNumber = sequenceNumber;
            MaterialDefinition = materialDefinition;
            ProductionTarget = productionTarget;
            Unit = unit;
            DueDate = dueDate;
            Duration = duration;
            StartTime = startTime;
            EndTime = endTime;
            ActuallyStartTime = actuallyStartTime;
            ActuallyEndTime = actuallyEndTime;
            WorkOrderStatus = workOrderStatus;
            ProductionParameterId = productionParameterId;
            QualityStandardId = qualityStandardId;
            SetupTime = setupTime;
            RunTime = runTime;
            ManufacturingOrderId = manufacturingOrderId;
            EquipmentId = equipmentId;
            PersonShift12 = personShift12;
            PersonShift3 = personShift3;

        }
    }



}
