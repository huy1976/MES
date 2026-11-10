namespace MesMicroservice.Infrastructure.Repositories.AuthModelRepositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void Create(User user)
        {
            _context.Users.Add(user);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _context.Users
                .ToListAsync();
        }

        public Task<User> GetUserById(string userId)
        {
            return _context.Users
                .FirstAsync(x => x.UserId == userId);
        }

        public Task<User?> GetUserByUsername(string username)
        {
            return _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(x => x.UserName == username);
        }
    }
}
