namespace MesMicroservice.Application.DTOs.UserDTOs
{
    public class AddPermissionsDTO
    {
        public Roles Role { get; set; }
        public int PageId { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
}
