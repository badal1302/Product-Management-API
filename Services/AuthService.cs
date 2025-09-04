
using ProductManagementApi.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            // Simple hardcoded users for demonstration
            // In production, this would come from a database
            _users = new List<User>
            {
                new User { Username = "admin", Password = "admin123", Role = "Admin" },
                new User { Username = "user", Password = "user123", Role = "User" }
            };
        }

    public bool ValidateCredentials(string username, string password)
        {
            var user = GetUser(username);
            return user != null && user.Password == password;
        }

        public User? GetUser(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public string? GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresInMinutes = int.TryParse(jwtSection["ExpiresInMinutes"], out var min) ? min : 60;
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                return null;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

