namespace MesMicroservice.Application.DTOs.MaterialDTOs
{
    public class MaterialDTO
    {
        public string MaterialId { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasure { get; set; }
        public List<OperationDTO> operationDTOs { get; set; } = new List<OperationDTO>();
        public MaterialDTO(string materialId, string name, string unitOfMeasure)
        {
            MaterialId = materialId;
            Name = name;
            UnitOfMeasure = unitOfMeasure;
        }
    }

    public class OperationDTO
    {
        public string OperationId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<SegmentDTO> segmentDTOs { get; set; } = new List<SegmentDTO>();
        public List<BomDTO> bomDTOs { get; set; } = new List<BomDTO>();
    }

    public class SegmentDTO
    {
        public string SegmentId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TypeSegment { get; set; } = string.Empty;

    }

    public class BomDTO
    {
        public string BomId { get; set; } = string.Empty;
        public string RawMaterialId { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
    }


}
