namespace MesMicroservice.Application.DTOs.DashboardDTOs;

public class MachineReportDTO
{
    public int TotalWorkOrders { get; set; } = 0;
    public int TotalMachines { get; set; } = 0;
    public List<DrawMachineDTO> DrawMachines { get; set; } = new List<DrawMachineDTO>();
    public List<EnamelMachineDTO> EnamelMachines { get; set; } = new List<EnamelMachineDTO>();
    public List<DrawEnamelMachineDTO> DrawEnamelMachines { get; set; } = new List<DrawEnamelMachineDTO>();
}

public class DrawMachineDTO
{
    public string MachineId { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public double TotalDraws { get; set; } = 0;
}

public class EnamelMachineDTO
{
    public string MachineId { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public double TotalEnamels { get; set; } = 0;
}

public class DrawEnamelMachineDTO
{
    public string MachineId { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public double TotalDrawEnamels { get; set; } = 0;
}