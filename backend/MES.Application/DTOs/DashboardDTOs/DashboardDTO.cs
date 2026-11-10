namespace MesMicroservice.Application.DTOs.DashboardDTOs
{
    public class DashboardDTO
    {
        public MOReportDTO MOReport { get; set; } = new MOReportDTO();
        public MaterialReportDTO MaterialReport { get; set; } = new MaterialReportDTO();
        public RawMaterialDTO RawMaterialReport { get; set; } = new RawMaterialDTO();
        public OeeDTO OeeReport { get; set; } = new OeeDTO();
        public MachineReportDTO MachineReport { get; set; } = new MachineReportDTO();
        public ErrorReportDTO ErrorReport { get; set; } = new ErrorReportDTO();
    }
}
