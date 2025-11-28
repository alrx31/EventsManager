using System;
using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Domain;
using EventManagement.Middlewares;

namespace EventManagement.Application.Services;

public class AuthService:IAuthService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    
    
    public AuthService(
        IParticipantRepository participantRepository, 
        IJwtService jwtService, 
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _participantRepository = participantRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }
    
    
    public async Task<LoginResponse> Login(LoginModel user)
    {
        var response = new LoginResponse();
        var identifyUser = await _participantRepository.GetParticipantByEmailAsync(user.Email);
        if (identifyUser is null ||
            (await _participantRepository.CheckPasswordAsync(identifyUser, user.Password)) == false)
        {
            return response;
        };
        response.IsLoggedIn = true;
        response.UserId = await _participantRepository.GetParticipantIdByEmailAsync(user.Email);
        response.JwtToken = _jwtService.GenerateJwtToken(identifyUser.Email);
        response.RefreshToken = _jwtService.GenerateRefreshToken();
        var identityUserTokenModel =
            await _participantRepository.getExtendedIdentityUserByEmailAsync(identifyUser.Email);
        
        if (identityUserTokenModel == null)
        {
            throw new ValidationException("User token record not found. Please contact support.");
        }
        
        identityUserTokenModel.RefreshToken = response.RefreshToken;
        identityUserTokenModel.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(12);
        await _participantRepository.UpdateRefreshTokenAsync(identityUserTokenModel);
        await _unitOfWork.CompleteAsync();
        return response;
    }

    public async Task<LoginResponse> RefreshToken(RefreshTokenModel refreshTokenModel)
    {
        var principal = _jwtService.GetTokenPrincipal(refreshTokenModel.JwtToken);
        var response = new LoginResponse();
        if (principal?.Identity?.Name is null) return response;
        var identityUser = await _participantRepository.getExtendedIdentityUserByEmailAsync(principal.Identity.Name);
        if (identityUser is null || string.IsNullOrEmpty(identityUser.RefreshToken) || identityUser.RefreshTokenExpiryTime < DateTime.UtcNow)
            return response;
        response.UserId = identityUser.Id;
        response.IsLoggedIn = true;
        response.JwtToken = _jwtService.GenerateJwtToken(identityUser.Email);
        response.RefreshToken = _jwtService.GenerateRefreshToken();
        var identityUserTokenModel =
            await _participantRepository.getExtendedIdentityUserByEmailAsync(identityUser.Email);
        
        if (identityUserTokenModel == null)
        {
            throw new ValidationException("User token record not found. Please contact support.");
        }
        
        identityUserTokenModel.RefreshToken = response.RefreshToken;
        identityUserTokenModel.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(12);
        await _participantRepository.UpdateRefreshTokenAsync(identityUserTokenModel);
        await _unitOfWork.CompleteAsync();
        return response;
    }

    public async Task Logout(LogoutModel model)
    {
        if (model.Token == null || model.UserId <= 0)
            throw new ValidationException("Invalid token or user id");
        await _participantRepository.CanselRefreshToken(model.UserId);
        await _unitOfWork.CompleteAsync();
    }
    
    public async Task<PasswordResetResponseDTO> ResetPasswordAsync(string email)
    {
        var response = new PasswordResetResponseDTO();
        
        if (string.IsNullOrWhiteSpace(email))
        {
            response.Success = false;
            response.Message = "Email обязателен";
            return response;
        }
        
        var user = await _participantRepository.GetParticipantByEmailAsync(email);
        
        if (user == null)
        {
            // Не раскрываем, существует ли пользователь (безопасность)
            response.Success = true;
            response.Message = "Если аккаунт с таким email существует, новый пароль будет отправлен на почту";
            return response;
        }
        
        // Генерируем новый пароль
        var newPassword = GenerateRandomPassword(10);
        
        // Обновляем пароль пользователя
        await _participantRepository.UpdatePasswordAsync(user, newPassword);
        await _unitOfWork.CompleteAsync();
        
        // Отправляем email с новым паролем
        try
        {
            await _emailService.SendPasswordResetEmailAsync(email, newPassword);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            // Продолжаем, даже если email не отправлен
        }
        
        response.Success = true;
        response.Message = "Если аккаунт с таким email существует, новый пароль будет отправлен на почту";
        return response;
    }
    
    private static string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
        var random = new Random();
        var password = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }
        
        return new string(password);
    }
    
}