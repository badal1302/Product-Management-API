using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using ProductManagementApi.Services;

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IAuthService _authService;

        public TestController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("public")]
        public IActionResult GetPublic()
        {
            return Ok(new { 
                Message = "This endpoint is public - no authentication required!",
                Timestamp = DateTime.UtcNow
            });
        }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtected()
        {
            var user = HttpContext.Items["User"];
            return Ok(new { 
                Message = "Basic Authentication successful!", 
                User = user,
                Timestamp = DateTime.UtcNow
            });
        }

    [HttpGet("users")]
    [Authorize]
        public IActionResult GetUsers()
        {
            // This is just for testing - in production you wouldn't expose user passwords
            return Ok(new { 
                Message = "Available users for testing",
                Note = "These are test users - don't use in production!",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
