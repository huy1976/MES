using MesMicroservice.Application.DTOs.UserDTOs;

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository; // Giả sử bạn có dịch vụ người dùng
        public AuthController(IMediator mediator, ITokenService tokenService, IUserRepository userRepository) : base(mediator)
        {
            _tokenService = tokenService;
            this._userRepository = userRepository;
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst("sub")?.Value; // lấy userId từ token
            if (userId == null) return Unauthorized();

            var user = await _userRepository.GetUserById(userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserId,
                user.UserName,
                user.Role
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO login)
        {
            var query = new CheckUserSignInQuery(login.Username, login.Password);
            var result = await RequestDtoAsync(query);

            if (result == null)
                return Unauthorized("Sai username hoặc password");

            var accessToken = _tokenService.GenerateAccessToken(result.UserName, result.Role);
            var refreshToken = _tokenService.GenerateRefreshToken(result.UserId, result.Role);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // true nếu dùng HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return Ok(new
            {
                AccessToken = accessToken
            });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var commad = new UpdateEquipmentStatusCommand();
            await _mediator.Send(commad);

            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized("Refresh token not found");
            var refreshToken = await _tokenService.ValidateRefreshTokenAsync(token);
            if (refreshToken == null)
                return Unauthorized("Invalid or expired refresh token");

            await _tokenService.LogoutAsync(token);
            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized("Refresh token not found");

            var refreshToken = await _tokenService.ValidateRefreshTokenAsync(token);
            if (refreshToken == null)
                return Unauthorized("Invalid or expired refresh token");

            // Thu hồi token cũ
            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            var newAccessToken = _tokenService.GenerateAccessToken(refreshToken.User.UserName, refreshToken.Role);
            var newRefreshToken = _tokenService.GenerateRefreshToken(refreshToken.UserId, refreshToken.Role);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }

        [HttpPost("CreateNewUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            return await RequestAsync(command);
        }

        [HttpPost("AddAddPermissions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPermissions([FromBody] List<AddPermissionsDTO> permissions)
        {
            var command = new AddPermissionsCommand{AddPermissionsDTOs = permissions};
            return await RequestAsync(command);
        }

        [HttpGet("GetPermissions")]
        public async Task<IActionResult> GetPermissionsByRole()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var token))
                return Unauthorized("Refresh token not found");

            var refreshToken = await _tokenService.ValidateRefreshTokenAsync(token);
            if (refreshToken == null)
                return Unauthorized("Invalid or expired refresh token");

            var query = new GetPermissionsByRoleQuery(refreshToken.Role.ToString());
            
            return await RequestAsync(query);
        }
    }
}
