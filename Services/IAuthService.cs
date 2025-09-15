using ProductManagementApi.Models;

namespace ProductManagementApi.Services
{
    public interface IAuthService
    {
        bool ValidateCredentials(string username, string password);
        User? GetUser(string username);
        string? GenerateJwtToken(User user);
    }
}
