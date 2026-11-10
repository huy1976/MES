namespace MesMicroservice.Application.Queries.Users
{
    public class GetPermissionsByRoleQueryHandler :IRequestHandler<GetPermissionsByRoleQuery, PermissionsDTO>
    {
        private readonly IPermissionRepository _permissionRepository;
        public GetPermissionsByRoleQueryHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<PermissionsDTO> Handle(GetPermissionsByRoleQuery request, CancellationToken cancellationToken)
        {
            var roleEnum = Enum.Parse<Roles>(request.Role, true);
            var permissions = await _permissionRepository.GetByRole(roleEnum);
            var permissionsDTO = new PermissionsDTO
            {
                role = request.Role,
                permissions = permissions.Select(p => new AddPermissionsDTO
                {
                    Role = p.Role,
                    PageId = p.PageId,
                    CanView = p.CanView,
                    CanEdit = p.CanEdit
                }).ToList()
            };
            return permissionsDTO;
        }
    }
}
