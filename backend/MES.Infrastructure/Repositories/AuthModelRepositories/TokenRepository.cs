namespace MesMicroservice.Infrastructure.Repositories.AuthModelRepositories
{
    public class TokenRepository : BaseRepository, ITokenRepository
    {
        public TokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void Create(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

        }

        public Task<RefreshToken?> GetRefreshTokenByToken(string token)
        {
            var refreshToken = _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
            return refreshToken;
        }

        public async Task<RefreshToken?> GetValidateRefreshTokenByToken(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);

        }

        public async Task RevokeAsync(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public void Update(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
