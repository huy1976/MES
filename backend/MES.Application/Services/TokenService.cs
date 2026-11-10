global using MesMicroservice.Domain.Enums;
global using MesMicroservice.Application.Interfaces;

namespace MesMicroservice.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        public TokenService(IConfiguration config, ITokenRepository tokenRepository, IUserRepository userRepository)
        {
            _config = config;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public string GenerateAccessToken(string userName, Roles role)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"]));

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Name, userName),
            new Claim("role", role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string userId, Roles role)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var user = _userRepository.GetUserById(userId).Result;
            var refreshToken = new RefreshToken(token: Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                                                userId: userId,
                                                role: role,
                                                expires: DateTime.Now.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationDays"])),
                                                user: user);

            _tokenRepository.Create(refreshToken);

            return refreshToken;
        }

        public string GenerateToken(Roles role)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Thông tin sẽ đưa vào JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "DemoUser"),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _tokenRepository.GetValidateRefreshTokenByToken(token);

            return refreshToken;
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _tokenRepository.RevokeAsync(refreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _tokenRepository.GetRefreshTokenByToken(refreshToken);

            if (token is null)
                throw new Exception("Invalid refresh token");

            await _tokenRepository.RevokeAsync(token);
        }
    }
}
