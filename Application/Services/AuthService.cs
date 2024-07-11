using System;
using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;

namespace EventManagement.Application.Services;

public class AuthService:IAuthService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    
    
    public AuthService(IParticipantRepository participantRepository, IJwtService jwtService, IUnitOfWork unitOfWork)
    {
        _participantRepository = participantRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
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
        
        identityUserTokenModel.RefreshToken = response.RefreshToken;
        identityUserTokenModel.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(12);
        await _participantRepository.UpdateRefreshTokenAsync(identityUserTokenModel);
        await _unitOfWork.CompleteAsync();
        return response;
    }

    public async Task Logout(LogoutModel model)
    {
        if (model.Token == null || model.UserId <= 0)
            throw new ArgumentException("Invalid token or user id");
        await _participantRepository.CanselRefreshToken(model.UserId);
        await _unitOfWork.CompleteAsync();
    }
    
}