namespace MesMicroservice.Application.Queries.Users
{
    public class CheckUserSignInQuery : IRequest<UserCheckDTO>
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public CheckUserSignInQuery(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
