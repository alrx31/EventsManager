using System.Threading.Tasks;
using EventManagement.Application.Models;

namespace EventManagement.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginModel user);
    Task<LoginResponse> RefreshToken(RefreshTokenModel refreshTokenModel);

}