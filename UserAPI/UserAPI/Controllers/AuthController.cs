using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAPI.Business.Interfaces;
using UserAPI.Business.Services;
using UserAPI.Model.Dto;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISessionManager _sessionManager;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        public AuthController(
            ISessionManager sessionManager,
            UserService userService,
            TokenService tokenService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _sessionManager = sessionManager;
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
            _configuration = configuration;
        }

        // Register Eklenmeli!!

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(model.Email, model.Password);
                if (user == null)
                {
                    _logger.LogWarning($"Failed login attempt for email: {model.Email}");
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var deviceInfo = Request.Headers["User-Agent"].ToString();
                var token = _tokenService.GenerateToken(user);
                var session = await _sessionManager.CreateSessionAsync(user.Id, deviceInfo);

                _logger.LogInformation($"Successful login for user: {user.Email}");

                return Ok(new
                {
                    Token = token,
                    ExpiresIn = Convert.ToInt32(
                        _configuration["JwtSettings:ExpirationInDays"]) * 24 * 60 * 60,

                    User = new LoginDto //Daha sonra Güncellenecek !!
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessions = await _sessionManager.GetUserSessionsAsync(userId);
            return Ok(sessions);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _sessionManager.InvalidateSessionAsync(token);
            return Ok();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _sessionManager.InvalidateAllSessionsAsync(userId);
            return Ok();
        }
    }
}
