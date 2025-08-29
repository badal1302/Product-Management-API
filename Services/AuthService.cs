using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductManagementApi.Models;

namespace ProductManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users;

        public AuthService()
        {
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
    }
}

