using System.Security.Claims;

namespace EventManagement.Application.Services;

public interface IJwtService
{
    string GenerateJwtToken(string value);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetTokenPrincipal(string token);
}