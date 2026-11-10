namespace MesMicroservice.Domain.AggregateModels.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(string userId);
        Task<List<User>> GetAllUsersAsync();


        void Create(User user);

    }
}
