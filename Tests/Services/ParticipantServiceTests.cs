using EventManagement.Application.Models;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Services;

public class ParticipantServiceTests
{
    private readonly Mock<IParticipantRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ParticipantService _participantService;

    public ParticipantServiceTests()
    {
        _mockRepository = new Mock<IParticipantRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Настройка Mock<IConfiguration> для возврата ключа JWT
        _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("your_secret_key");

        _participantService = new ParticipantService(_mockRepository.Object, _mockConfiguration.Object, _mockUnitOfWork.Object);
    }
    
    // регистрация
    [Fact]
    // Успешная регистрация
    public async Task Register_Success()
    {
        // Arrange
        var ParticipantRegisterDTO = new ParticipantRegisterDTO
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            RegistrationDate = DateTime.Now,
            Email = "blabla@gmail.com",
            Password = "123456"
        };
        
        
        _mockRepository.Setup(repo => repo.RegisterParticipantAsync(ParticipantRegisterDTO))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(repo => repo.AddRefreshTokenField(ParticipantRegisterDTO))
            .Returns(Task.CompletedTask);

        // Act
        await _participantService.RegisterParticipantAsync(ParticipantRegisterDTO);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
                
    }

   
    [Fact]
    // Неудачная регистрация в репозитории
    public async Task Register_Fail()
    {
        // Arrange
        var participant = new ParticipantRegisterDTO
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = DateTime.Parse("1990-01-01"),
            RegistrationDate = DateTime.UtcNow,
            Email = "john.doe@example.com",
            Password = "password"
        };

        _mockRepository.Setup(repo => repo.RegisterParticipantAsync(participant))
            .ThrowsAsync(new Exception("Failed to register participant"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _participantService.RegisterParticipantAsync(participant));
        Assert.Equal("Failed to register participant", exception.Message);
    }
    
    // регистрация в мероприятии
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(15, 15)]
    // Успешная регистрация
    public async Task RegisterParticipantToEvent_Success(int Eid, int id)
    {
        // Arrange
        var eventId = Eid;
        var participantId = id;
        
        // Act
        await _participantService.RegisterParticipantToEventAsync(eventId, participantId);
        
        // assets
        _mockRepository.Verify(repo => repo.RegisterParticipantToEventAsync(eventId, participantId), Times.Once);
        
    }
    
    
    [Theory]
    [InlineData(0,1)]
    [InlineData(1,0)]
    [InlineData(-1,-1)]
    // Неудачная регистрация из-за нулевого поля
    public async Task RegisterParticipantToEvent_Fail_Fields(int Eid,int id)
    {
        // Arrange
        var eventId = Eid;
        var participantId =id;
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _participantService.RegisterParticipantToEventAsync(eventId, participantId));
    }
    
    
    [Fact]
    // Неудачная регистрация в репозитории
    public async Task RegisterParticipantToEvent_Fail()
    {
        // Arrange
        var eventId = 1;
        var participantId = 1;

        _mockRepository.Setup(repo => repo.RegisterParticipantToEventAsync(eventId, participantId))
            .ThrowsAsync(new Exception("Failed to register participant to event"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _participantService.RegisterParticipantToEventAsync(eventId, participantId));
        Assert.Equal("Failed to register participant to event", exception.Message);
    }
    
    
    // отмена регистрации на событие
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(15, 15)]
    // Успешная отмена регистрации
    public async Task CancelRegistration_Success(int Eid, int id)
    {
        // Arrange
        var eventId = Eid;
        var participantId = id;
        
        // Act
        await _participantService.CancelRegistrationAsync(eventId, participantId);
        
        // assets
        _mockRepository.Verify(repo => repo.CancelRegistrationAsync(eventId, participantId), Times.Once);
        
    }
    
    [Theory]
    [InlineData(0,1)]
    [InlineData(1,0)]
    [InlineData(-1,-1)]
    // Неудачная отмена регистрации из-за нулевого поля
    public async Task CancelRegistration_Fail_Fields(int Eid,int id)
    {
        // Arrange
        var eventId = Eid;
        var participantId =id;
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _participantService.CancelRegistrationAsync(eventId, participantId));
    }
    
    [Fact]
    // Неудачная отмена регистрации в репозитории
    public async Task CancelRegistration_Fail()
    {
        // Arrange
        var eventId = 1;
        var participantId = 1;

        _mockRepository.Setup(repo => repo.CancelRegistrationAsync(eventId, participantId))
            .ThrowsAsync(new Exception("Failed to cancel registration"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _participantService.CancelRegistrationAsync(eventId, participantId));
        Assert.Equal("Failed to cancel registration", exception.Message);
    }
    
    
    // получение пользователей по id мероприятия
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(15)]
    // Успешное получение пользователей
    public async Task GetParticipantsByEventId_Success(int Eid)
    {
        // Arrange
        var eventId = Eid;
        
        // Act
        await _participantService.GetParticipantsByEventIdAsync(eventId);
        
        // assets
        _mockRepository.Verify(repo => repo.GetParticipantsByEventIdAsync(eventId), Times.Once);
        
    }
    
    
    [Fact]
    // Неудачное получение пользователей в репозитории
    public async Task GetParticipantsByEventId_Fail()
    {
        // Arrange
        var eventId = 1;

        _mockRepository.Setup(repo => repo.GetParticipantsByEventIdAsync(eventId))
            .ThrowsAsync(new Exception("No Participants Found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _participantService.GetParticipantsByEventIdAsync(eventId));
        Assert.Equal("No Participants Found", exception.Message);
    }
    
    
    
}