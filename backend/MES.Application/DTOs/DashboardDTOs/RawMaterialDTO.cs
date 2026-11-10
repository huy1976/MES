namespace MesMicroservice.Application.DTOs.DashboardDTOs
{
    public class RawMaterialDTO
    {
        public double TotalRawMaterial { get; set; }
        public List<ListRawMaterialInMO> RawMaterialsInMOs { get; set; } = new List<ListRawMaterialInMO>();

    }

    public class ListRawMaterialInMO
    {
        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public double Quantity { get; set; } = 0;
        public double RateInMO { get; set; } = 0;
        public string Unit { get; set; } = string.Empty;
    }
}
