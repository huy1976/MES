namespace MesMicroservice.Algorithm.Interfaces
{
    public interface IEquipmentSelectorService
    {
        Task<List<Equipment>> GetEligibleEquipmentsAsync(int workOrderId);
        Task AssignEligibleMachinesToJobsAsync(Dictionary<string, List<Job>> jobs);
    }
}
