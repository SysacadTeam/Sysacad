using System.Security.Claims;

namespace Sysacad.Server.Services
{
    public interface IJwtService
    {
        string GenerateToken(int userId);
        ClaimsPrincipal? ValidateToken(string token);
        bool ShouldRefreshToken(string token);
        string? RefreshToken(string token);
    }
}