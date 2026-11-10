namespace MesMicroservice.Domain.AggregateModels.UserAggregate
{
    public interface ITokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetValidateRefreshTokenByToken(string token);
        void Create(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
        Task RevokeAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenByToken(string token);
    }
}
