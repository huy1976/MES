namespace MesMicroservice.Application.Commands.Users
{
    [DataContract]
    public class CreateUserCommand : IRequest<bool>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? PersonId { get; set; }
        public Roles Role { get; set; }

        public CreateUserCommand(string userName, string password, string? personId, Roles role)
        {
            UserName = userName;
            Password = password;
            PersonId = personId;
            Role = role;
        }
    }
}
