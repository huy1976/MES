using MesMicroservice.Domain.Enums;

namespace MesMicroservice.Application.DTOs.UserDTOs
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PersonId { get; set; }

        public UserDTO(string userName, string password, string personId)
        {
            UserName = userName;
            Password = password;
            PersonId = personId;
        }
    }

    public class UserCheckDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Roles Role { get; set; }

        public UserCheckDTO(string userId, string userName, Roles role)
        {
            UserId = userId;
            UserName = userName;
            Role = role;
        }
    }
}
