namespace MesMicroservice.Application.DTOs.ManufacturingOrderDTOs
{
    public class ManufacturingOrderDTO
    {
        public string ManufacturingOrderId { get; set; }
        public string MaterialDefinitionId { get; set; }
        public string MaterialDefinitionName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerId { get; set; }
        public string OperationsDefinitionId { get; set; }
        public string QualityStandardId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public List<WorkOrderViewDTO> workOrderDTOs { get; set; } = new List<WorkOrderViewDTO>();

        public ManufacturingOrderDTO(string manufacturingOrderId, string materialDefinitionId, string materialDefinitionName, decimal quantity, string unit, DateTime dueDate, string customerId, string operationsDefinitionId, string qualityStandardId, string status, string createdBy, DateTime createDate)
        {
            ManufacturingOrderId = manufacturingOrderId;
            MaterialDefinitionId = materialDefinitionId;
            MaterialDefinitionName = materialDefinitionName;
            Quantity = quantity;
            Unit = unit;
            DueDate = dueDate;
            CustomerId = customerId;
            OperationsDefinitionId = operationsDefinitionId;
            QualityStandardId = qualityStandardId;
            Status = status;
            CreatedBy = createdBy;
            CreateDate = createDate;
        }
    }
}
