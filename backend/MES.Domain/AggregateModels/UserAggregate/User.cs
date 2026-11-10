namespace MesMicroservice.Domain.AggregateModels.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public int? PersonId { get; set; }
        public Person? Person { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public User(string userId, string userName, string password, Roles role)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
            Role = role;
        }

        public void addPerson(Person person)
        {
            Person = person;
        }
    }
}
