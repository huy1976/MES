namespace MesMicroservice.Application.DTOs.WorkOrderDTOs
{
    public class WorkOrderDTO
    {
        public string WorkOrderId { get; private set; }
        public string SegmentId { get; private set; }
        public int SequenceNumber { get; set; }
        public string ManufacturingOrderId { get; private set; }
        public int OperationsSegmentId { get; private set; }
        public string OperationsSegmentName { get; private set; }
        public string MaterialDefinitionId { get; set; }
        public decimal ProductionTarget { get; private set; }
        public string Unit { get; private set; }
        public string ProductionParamId { get; set; }
        public string QualityParamId { get; set; }
        public List<string> BOMIds { get; set; } = new List<string>();

        public WorkOrderDTO(string workOrderId, string segmentId, int sequenceNumber, string manufacturingOrderId, int operationsSegmentId, string operationsSegmentName, string materialDefinitionId, decimal productionTarget, string unit, string productionParamId, string qualityParamId, List<string> bOMIds)
        {
            WorkOrderId = workOrderId;
            SegmentId = segmentId;
            SequenceNumber = sequenceNumber;
            ManufacturingOrderId = manufacturingOrderId;
            OperationsSegmentId = operationsSegmentId;
            OperationsSegmentName = operationsSegmentName;
            MaterialDefinitionId = materialDefinitionId;
            ProductionTarget = productionTarget;
            Unit = unit;
            ProductionParamId = productionParamId;
            QualityParamId = qualityParamId;
            BOMIds = bOMIds;
        }
    }
}
