namespace MesMicroservice.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Roles role);
        string GenerateAccessToken(string userId, Roles role);
        RefreshToken GenerateRefreshToken(string userId, Roles role);
        Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}
