using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Models;
using ProductManagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using ProductManagementApi.Attributes;

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
        [AllowAnonymous] // Login endpoint should be accessible without authentication
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

        [HttpGet("users")]
        [RequireAdmin] // Only Admin can view all users
        public IActionResult GetAllUsers()
        {
            // In a real application, you would get this from a user service
            return Ok(new { 
                message = "User list endpoint - requires Admin role",
                users = new[] { 
                    new { username = "admin", role = "Admin" },
                    new { username = "user", role = "User" }
                }
            });
        }

        [HttpPost("users")]
        [RequireAdmin] // Only Admin can create new users
        public IActionResult CreateUser([FromBody] User newUser)
        {
            // In a real application, you would create the user here
            return Ok(new { 
                message = "User creation endpoint - requires Admin role", 
                user = newUser 
            });
        }

        [HttpPut("users/{username}")]
        [RequireAdmin] // Only Admin can update users
        public IActionResult UpdateUser(string username, [FromBody] User updatedUser)
        {
            // In a real application, you would update the user here
            return Ok(new { 
                message = "User update endpoint - requires Admin role", 
                username, 
                user = updatedUser 
            });
        }

        [HttpDelete("users/{username}")]
        [RequireAdmin] // Only Admin can delete users
        public IActionResult DeleteUser(string username)
        {
            // In a real application, you would delete the user here
            return Ok(new { 
                message = "User deletion endpoint - requires Admin role", 
                username 
            });
        }

        [HttpGet("profile")]
        [RequireUserOrAdmin] // Both User and Admin can view their own profile
        public IActionResult GetProfile()
        {
            // In a real application, you would get the current user's profile
            var username = User.Identity?.Name ?? "Unknown";
            return Ok(new { 
                message = "Profile endpoint - requires User or Admin role", 
                username 
            });
        }
    }
}
