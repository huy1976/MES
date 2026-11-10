namespace MesMicroservice.Domain.AggregateModels.UserAggregate
{
    public class RefreshToken : IAggregateRoot
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Roles Role { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RefreshToken()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public RefreshToken(string token, string userId, Roles role, DateTime expires, User user)
        {
            Token = token;
            UserId = userId;
            Role = role;
            Expires = expires;
            User = user;
        }
    }
}
