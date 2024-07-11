using System.Security.Claims;
using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;
using Moq;

namespace Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IParticipantRepository> _mockParticipantRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockParticipantRepository = new Mock<IParticipantRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _authService = new AuthService(_mockParticipantRepository.Object, _mockJwtService.Object,
            _mockUnitOfWork.Object);
    }

    // авторизация
    [Fact]
    // Тест на успешный логин
    public async Task Login_Success()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "admin",
            Password = "admin"
        };
        var extendedIdentityUser = new ExtendedIdentityUser
        {
            Email = "admin",
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.UtcNow
        };

        _mockParticipantRepository.Setup(x => x.GetParticipantByEmailAsync(loginModel.Email))
            .ReturnsAsync(loginModel);

        _mockParticipantRepository.Setup(x => x.CheckPasswordAsync(loginModel, loginModel.Password))
            .ReturnsAsync(true);

        _mockJwtService.Setup(jwt => jwt.GenerateJwtToken(loginModel.Email))
            .Returns("jwt_token");
        
        _mockJwtService.Setup(jwt => jwt.GenerateRefreshToken())
            .Returns("refresh_token");
        _mockParticipantRepository.Setup(repo => repo.getExtendedIdentityUserByEmailAsync(loginModel.Email))
            .ReturnsAsync(extendedIdentityUser);
        
        // Act

        var response = await _authService.Login(loginModel);
        
        // Assert
        
        Assert.True(response.IsLoggedIn);
        Assert.Equal("jwt_token", response.JwtToken);
        Assert.Equal("refresh_token", response.RefreshToken);
    }

    [Fact]
    // Тест на неудачный логин ( неверный пароль)
    public async Task Login_Fail_Password()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "admin",
            Password = "admin1"
        };
        
        _mockParticipantRepository.Setup(x => x.GetParticipantByEmailAsync(loginModel.Email))
            .ReturnsAsync(loginModel);

        _mockParticipantRepository.Setup(x => x.CheckPasswordAsync(loginModel, loginModel.Password))
            .ReturnsAsync(false);

        // Act

        var response = await _authService.Login(loginModel);
        
        // Assert
        
        Assert.False(response.IsLoggedIn);
        
    }
    
    [Fact]
    // Тест на неудачный логин (неверный email)
    public async Task Login_Fail_Email()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "admin1",
            Password = "admin"
        };
        
        _mockParticipantRepository.Setup(x => x.GetParticipantByEmailAsync(loginModel.Email))
            .ReturnsAsync((LoginModel)null);

        // Act

        var response = await _authService.Login(loginModel);
        
        // Assert
        
        Assert.False(response.IsLoggedIn);
        
    }
    
    
    // обновление токена
    [Fact]
    // Тест на успешное обновление токена
    public async Task RefreshToken_Success()
    {
        // Arrange

        var refreshTokenModel = new RefreshTokenModel { JwtToken = "jwt_token", RefreshToken = "old_refresh_token" };
        var participantEmail = "admin";
        var identityUser = new ExtendedIdentityUser
        {
            Email = participantEmail,
            RefreshToken = "old_refresh_token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5)
        };

        _mockJwtService.Setup(jwt => jwt.GetTokenPrincipal(refreshTokenModel.JwtToken))
            .Returns(new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, participantEmail) })));

        _mockParticipantRepository.Setup(repo => repo.getExtendedIdentityUserByEmailAsync(participantEmail))
            .ReturnsAsync(identityUser);

        _mockJwtService.Setup(jwt => jwt.GenerateJwtToken(participantEmail))
            .Returns("new_jwt_token");

        _mockJwtService.Setup(jwt => jwt.GenerateRefreshToken())
            .Returns("new_refresh_token");

        // act

        var response = await _authService.RefreshToken(refreshTokenModel);

        // assert

        Assert.True(response.IsLoggedIn);
        Assert.Equal("new_jwt_token", response.JwtToken);
        Assert.Equal("new_refresh_token", response.RefreshToken);
        
        _mockParticipantRepository.Verify(repo => repo.UpdateRefreshTokenAsync(It.Is<ExtendedIdentityUser>(u => u.RefreshToken == "new_refresh_token")), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        
    }

    [Fact]
    // Тест на неудачное обновление токена (refresh token истек или не найден)
    public async Task ResreshToken_Fail_TimeLimit()
    {
        // Arrange
        var refreshTokenModel = new RefreshTokenModel { JwtToken = "jwt_token", RefreshToken = "old_refresh_token" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "test@example.com") }));
        var identityUser = new ExtendedIdentityUser
        {
            Email = "test@example.com",
            RefreshToken = "old_refresh_token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1)
        };

        _mockJwtService.Setup(jwt => jwt.GetTokenPrincipal(refreshTokenModel.JwtToken))
            .Returns(principal);

        _mockParticipantRepository.Setup(repo => repo.getExtendedIdentityUserByEmailAsync(principal.Identity.Name))
            .ReturnsAsync(identityUser);

        // Act
        var response = await _authService.RefreshToken(refreshTokenModel);

        // Assert
        Assert.False(response.IsLoggedIn);
        Assert.Null(response.JwtToken);
        Assert.Null(response.RefreshToken);
    }

    [Fact]
    // Тест на неудачное обновление токена (пользователь не найден)
    public async Task ResreshToken_Fail_UserNotFound()
    {
        // Arrange
        var refreshTokenModel = new RefreshTokenModel { JwtToken = "jwt_token", RefreshToken = "old_refresh_token" };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "test@example.com") }));

        _mockJwtService.Setup(jwt => jwt.GetTokenPrincipal(refreshTokenModel.JwtToken))
            .Returns(principal);

        _mockParticipantRepository.Setup(repo => repo.getExtendedIdentityUserByEmailAsync(principal.Identity.Name))
            .ReturnsAsync((ExtendedIdentityUser)null);

        // Act
        var response = await _authService.RefreshToken(refreshTokenModel);

        // Assert
        Assert.False(response.IsLoggedIn);
        Assert.Null(response.JwtToken);
        Assert.Null(response.RefreshToken);
    }
    
    
    // выход из системы
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(15)]
    // Тест на успешный выход из системы
    public async Task Logout_Success(int id)
    {
        // Arrange
        var logoutModel = new LogoutModel
        {
            UserId = id,
            Token = "blabla"
        };
        _mockParticipantRepository.Setup(repo => repo.CanselRefreshToken(logoutModel.UserId))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _authService.Logout(logoutModel);

        _mockParticipantRepository.Verify(repo => repo.CanselRefreshToken(logoutModel.UserId), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
    
    [Fact]
    // Тест на неудачный выход из системы (неверный токен)
    public async Task Logout_Fail_Token()
    {
        // Arrange
        var logoutModel = new LogoutModel
        {
            UserId = 1,
            Token = null
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _authService.Logout(logoutModel));
        
        // Assert
        Assert.Equal("Invalid token or user id", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-15)]
    // Тест на неудачный выход из системы (неверный id пользователя)
    public async Task Logout_Fail_UserId(int id)
    {
        // Arrange
        var logoutModel = new LogoutModel
        {
            UserId = id,
            Token = "blabla"

        };
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _authService.Logout(logoutModel));
        
        // Assert
        Assert.Equal("Invalid token or user id", exception.Message);

        
    }
}
    
    
