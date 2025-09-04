using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Models;
using ProductManagementApi.Services;

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            if (!_authService.ValidateCredentials(login.Username, login.Password))
                return Unauthorized(new { message = "Invalid username or password" });

            var user = _authService.GetUser(login.Username);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var token = _authService.GenerateJwtToken(user);
            if (token == null)
                return StatusCode(500, new { message = "Token generation failed" });

            return Ok(new { token });
        }
    }
}
