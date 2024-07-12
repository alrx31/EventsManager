using EventManagement.Application;
using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests.Services;

public class EventServiceTests
{
    private readonly EventService _eventService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEventRepository> _mockRepository;

    public EventServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepository = new Mock<IEventRepository>();
        _eventService = new EventService(_mockRepository.Object, _mockUnitOfWork.Object);
    }
    
    // получение всех мероприятий ( с пагинацией)
    
    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 2)]
    [InlineData(3, 1)]
    // успешное получение всех мероприятий
    public async Task GetAllEventsAsync_Success(int page, int pageSize)
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllEventsAsync(page, pageSize))
            .ReturnsAsync(new List<EventRequest>
            {
                new EventRequest
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    Date = new DateTime(2022, 1, 1),
                    Location = "Location1",
                    Category = "Category1",
                    MaxParticipants = 100,
                    ImageSrc = "ImageSrc1"
                },
                new EventRequest
                {
                    Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    Date = new DateTime(2022, 2, 2),
                    Location = "Location2",
                    Category = "Category2",
                    MaxParticipants = 200,
                    ImageSrc = "ImageSrc2"
                }
            });

        // Act
        var result = await _eventService.GetAllEventsAsync(page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
   
    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 0)]
    [InlineData(-1, -1)]
    // неудачное получение всех мероприятий из-за нулевого поля
    public async Task GetAllEventsAsync_Fail_Fields(int page, int pageSize)
    {
        // Arrange
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetAllEventsAsync(page, pageSize));
    }
    
    [Fact]
    // неудачное получение всех мероприятий из-за отсутствия мероприятий
    public async Task GetAllEventsAsync_Fail()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetAllEventsAsync(1, 5))
            .ReturnsAsync((IEnumerable<EventRequest>)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetAllEventsAsync(1, 5));
    }
    
    
    // получение одного мероприятия
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    // успешное получение одного мероприятия
    public async Task GetEventByIdAsync_Success(int id)
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetEventByIdAsync(id))
            .ReturnsAsync(new Event
            {
                Id = id,
                Name = "Event1",
                Description = "Description1",
                Date = new DateTime(2022, 1, 1),
                Location = "Location1",
                Category = "Category1",
                MaxParticipants = 100,
                ImageData = new byte[2]
            });

        // Act
        var result = await _eventService.GetEventByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    // неудачное получение одного мероприятия из-за нулевого поля
    public async Task GetEventByIdAsync_Fail_Fields(int id)
    {
        // Arrange
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByIdAsync(id));
    }
    
    [Fact]
    // неудачное получение одного мероприятия из-за отсутствия мероприятия
    public async Task GetEventByIdAsync_Fail()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetEventByIdAsync(1))
            .ReturnsAsync((Event)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByIdAsync(1));
    }
    
    
    
    // получение одного мероприятия по запросу
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    // успешное получение одного мероприятия по запросу
    public async Task GetEventByIdAsyncRequest_Success(int id)
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetEventByIdAsyncRequest(id))
            .ReturnsAsync(new EventRequest
            {
                Id = id,
                Name = "Event1",
                Description = "Description1",
                Date = new DateTime(2022, 1, 1),
                Location = "Location1",
                Category = "Category1",
                MaxParticipants = 100,
                ImageSrc = "ImageSrc1"
            });

        // Act
        var result = await _eventService.GetEventByIdAsyncRequest(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    // неудачное получение одного мероприятия по запросу из-за нулевого поля
    public async Task GetEventByIdAsyncRequest_Fail_Fields(int id)
    {
        // Arrange
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByIdAsyncRequest(id));
    }
    
    
    [Fact]
    // неудачное получение одного мероприятия по запросу из-за отсутствия мероприятия
    public async Task GetEventByIdAsyncRequest_Fail()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetEventByIdAsyncRequest(1))
            .ReturnsAsync((EventRequest)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByIdAsyncRequest(1));
    }
    
    
    
    // получение мероприятия по имени
    
    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("nametest")]
    // успешное получение мероприятия по имени
    public async Task GetEventByNameAsync_Success(string name)
    {
        // Arrange
        var Event = new Event();
        
        _mockRepository.Setup(repo => repo.GetEventByNameAsync(name))
            .ReturnsAsync(Event);
        
        
        // Act
        var result = await _eventService.GetEventByNameAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Event, result);
    }
    
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    // неудачное получение мероприятия по имени из-за нулевого поля
    public async Task GetEventByNameAsync_Fail_Fields(string name)
    {
        // Arrange
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByNameAsync(name));
    }
    
    [Fact]
    // неудачное получение мероприятия по имени из-за отсутствия мероприятия
    public async Task GetEventByNameAsync_Fail()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetEventByNameAsync("name"))
            .ReturnsAsync((Event)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByNameAsync("name"));
    }
    
    
    // создание мероприятия
    
    [Fact]
    // успешное создание мероприятия
    public async Task AddEventAsync_Success()
    {
        // Arrange
        var newEvent = new EventDTO
        {
            Name = "Event1",
            Description = "Description1",
            Date = new DateTime(2022, 1, 1),
            Location = "Location1",
            Category = "Category1",
            MaxParticipants = 100,
            ImageData = new FormFile(new MemoryStream(new byte[2]), 0, 2, "ImageSrc1", "ImageSrc1")
        };

        // Act
        await _eventService.AddEventAsync(newEvent);

        // Assert
        _mockRepository.Verify(repo => repo.AddEventAsync(newEvent), Times.Once);
    }

    [Fact]
    // неудачное создание мероприятия
    public async Task AddEventAsync_Fail_NullField()
    {
        var Event = new EventDTO();
        await Assert.ThrowsAsync<Exception>(() => _eventService.AddEventAsync(Event));
    }
    
    [Fact]
    // неудачное создание мероприятия в репозитории
    public async Task AddEventAsync_Fail()
    {
        // Arrange
        var Event = new EventDTO();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _eventService.GetEventByNameAsync("name"));
    }
    
    
    
    
    
    // обновление мероприятия
    
    
    // удаление мероприятия
    
    // получение всех мероприятий по критериям   
    
    // получение всех мероприятий у пользователя
    
    // поиск мероприятия (с пагинацией)
    
}