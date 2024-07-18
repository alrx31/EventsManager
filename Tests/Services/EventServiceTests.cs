using EventManagement.Application;
using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure;
using EventManagement.Domain;
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

    
    
    
    
    
    
    // обновление мероприятия
    [Fact]
    // удачное обновление мероприятия
    public async Task UpdateEvent_Success()
    {
        var EventId = 1;
        var newEvent = new EventDTO
        {
            Name = "1",
            Description = "1",
            Date = new DateTime(2022, 1, 1),
            Location = "1",
            Category = "1",
            MaxParticipants = 1,
            ImageData = new FormFile(new MemoryStream(new byte[2]), 0, 2, "1", "1")
        };

        await _eventService.UpdateEventAsync(EventId, newEvent);
        
        _mockRepository.Verify(repo => repo.UpdateEventAsync(EventId, newEvent), Times.Once);
    }
 
    [Fact]
    // неудачное обновление мероприятия в репозитории
    
    public async Task UpdateEvent_Fail()
    {
        var EventId = 1;
        var newEvent = new EventDTO
        {
            Name = "1",
            Description = "1",
            Date = new DateTime(2022, 1, 1),
            Location = "1",
            Category = "1",
            MaxParticipants = 1,
            ImageData = new FormFile(new MemoryStream(new byte[2]), 0, 2, "1", "1")
        };

        _mockRepository.Setup(repo => repo.UpdateEventAsync(EventId, newEvent))
            .ThrowsAsync(new Exception("Failed to update event"));

        var exception = await Assert.ThrowsAsync<Exception>(() => _eventService.UpdateEventAsync(EventId, newEvent));
        Assert.Equal("Failed to update event", exception.Message);
    }

    // удаление мероприятия

    [Fact]
    // успешное удаление мероприятия
    public async Task DeleteEvent_Success()
    {
        // Arrange
        int eventId = 1;
        var eventToDelete = new Event { Id = eventId };
        _mockRepository.Setup(repo => repo.GetEventByIdAsync(eventId))
            .ReturnsAsync(eventToDelete);
        _mockRepository.Setup(repo => repo.DeleteEventAsync(eventId))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _eventService.DeleteEventAsync(eventId);

        // Assert
        _mockRepository.Verify(repo => repo.GetEventByIdAsync(eventId), Times.Once);
        _mockRepository.Verify(repo => repo.DeleteEventAsync(eventId), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

  
    [Fact]
    // успешное получение всех мероприятий по критериям
    public async Task GetEventsByCriteriaAsync_Success()
    {
        // Arrange
        var criteria = new EventCriteria { Location = "Test Event" };
        int page = 1;
        int pageSize = 10;

        var events = new List<EventRequest>
        {
            new EventRequest { Id = 1, Name = "Test Event 1" },
            new EventRequest { Id = 2, Name = "Test Event 2" }
        };

        _mockRepository.Setup(repo => repo.GetEventsByCriteriaAsync(criteria, page, pageSize))
            .ReturnsAsync(events);

        // Act
        var result = await _eventService.GetEventsByCriteriaAsync(criteria, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(events, result);
    }
    // получение всех мероприятий у пользователя
    
    [Fact]
    // успешное получение всех мероприятий у пользователя
    public async Task getEventsByUserId_Fail()
    {
        // Arrange
        int invalidUserId = 0;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _eventService.getEventsByUserId(invalidUserId));
        Assert.Equal("Invalid Id", exception.Message);
    }

    [Fact]
    // успешное получение всех мероприятий у пользователя
    public async Task getEventsByUserId_Success()
    {
        // Arrange
        int validUserId = 1;
        var events = new List<EventRequest>
        {
            new EventRequest { Id = 1, Name = "Event 1" },
            new EventRequest { Id = 2, Name = "Event 2" }
        };

        _mockRepository.Setup(repo => repo.getEventsByUserId(validUserId))
            .ReturnsAsync(events);

        // Act
        var result = await _eventService.getEventsByUserId(validUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(events, result);
    }
    // поиск мероприятия (с пагинацией)
    
    
    [Fact]
    // успешный поиск мероприятия
    public async Task SearchEvents_Success()
    {
        // Arrange
        var model = new SearchDTO { Date = DateTime.Now, Name = "Test Event" };
        int page = 1;
        int pageSize = 10;

        var events = new List<EventRequest>
        {
            new EventRequest { Id = 1, Name = "Test Event 1" },
            new EventRequest { Id = 2, Name = "Test Event 2" }
        };

        _mockRepository.Setup(repo => repo.SearchEvents(model, page, pageSize))
            .ReturnsAsync(events);

        // Act
        var result = await _eventService.SearchEvents(model, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(events, result);
    }
    
}