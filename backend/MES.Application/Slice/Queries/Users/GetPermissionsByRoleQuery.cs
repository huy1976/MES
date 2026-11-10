namespace MesMicroservice.Application.Queries.Users
{
    public class GetPermissionsByRoleQuery : IRequest<PermissionsDTO>
    {
        public string Role { get; set; }
        public GetPermissionsByRoleQuery(string role)
        {
            Role = role;
        }
    }
}
