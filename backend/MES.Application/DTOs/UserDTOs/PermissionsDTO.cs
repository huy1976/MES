namespace MesMicroservice.Application.DTOs.UserDTOs
{
    public class PermissionsDTO
    {
        public string role { get; set; } = string.Empty;    
        public List<AddPermissionsDTO> permissions { get; set; } = new List<AddPermissionsDTO>();
    }
}
