namespace MesMicroservice.Algorithm.Interfaces
{
    public interface IPersonnelSelectorService
    {
        Task<Schedule> SelectPersonnel(Schedule schedule);
    }
}
