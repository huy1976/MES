namespace MesMicroservice.Application.Commands.Users
{
    public class AddPermissionsCommand : IRequest<bool>
    {
        public List<AddPermissionsDTO> AddPermissionsDTOs { get; set; } = new List<AddPermissionsDTO>();
    }
}
