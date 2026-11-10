namespace MesMicroservice.Application.Commands.Users
{
    public class AddPermissionsCommandHandler :IRequestHandler<AddPermissionsCommand, bool>
    {
        private readonly IPermissionRepository _permissionRepository;

        public AddPermissionsCommandHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> Handle(AddPermissionsCommand request, CancellationToken cancellationToken)
        {
            var permissions = request.AddPermissionsDTOs.Select(dto => new Permission
            {
                Role = dto.Role,
                PageId = dto.PageId,
                CanView = dto.CanView,
                CanEdit = dto.CanEdit
            }).ToList();

            return await _permissionRepository.AddOrUpdatePermissionsAsync(permissions);
        }
    }
}
