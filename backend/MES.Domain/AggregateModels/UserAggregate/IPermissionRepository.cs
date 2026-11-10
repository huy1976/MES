namespace MesMicroservice.Domain.AggregateModels.UserAggregate
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission?> GetByIdAsync(string permissionId);
        Task<bool> HasViewPermissionAsync(Roles role, int pageId);
        Task<bool> HasEditPermissionAsync(Roles role, int pageId);
        Task<bool> AddOrUpdatePermissionsAsync(List<Permission> permissions);
        Task<List<Permission>> GetByRole(Roles role);

    }
}
