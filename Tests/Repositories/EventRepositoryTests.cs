using Xunit;
using Moq;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application;
using EventManagement.Application.Models;
using EventManagement.Domain;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using EventManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class EventRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    
    private DbContextOptions<ApplicationDbContext> _options;
    
    public EventRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        _context = new ApplicationDbContext(_options);

        var config = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
        _eventRepository = new EventRepository(_context, _mapper);

        SeedDatabase(_context);
    }
    
    private void SeedDatabase(ApplicationDbContext _context)
    {
        _context.Events.RemoveRange(_context.Events);
        _context.SaveChanges();
        
        _context.Events.Add(new Event
        {
            Id = 1,
            Name = "Test Event",
            Description = "Description",
            Location = "Location",
            Category = "Category",
            Date = DateTime.UtcNow,
            MaxParticipants = 100,
            ImageData = new byte[2]
        });
        _context.SaveChanges();

        _context.Events.Add(new Event
        {
            Id = 2,
            Name = "Another Event",
            Description = "Another Description",
            Location = "Another Location",
            Category = "Another Category",
            Date = DateTime.UtcNow,
            MaxParticipants = 200,
            ImageData = new byte[2]
        });
        _context.SaveChanges();
    }
    
    
    // получение всех мероприятий
    
    [Fact]
    // получение 1 мероприятия
    public async Task GetAllEventsAsync_ShouldReturnPagedResults()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _eventRepository = new EventRepository(_context,_mapper);
        // Act
        var result = await _eventRepository.GetAllEventsAsync(2, 1);
        // Assert
        Assert.Single(result);
        Assert.Equal("Test Event",result.First().Name);
    }

    [Fact]
    // получение всех мероприятий
    public async Task GetAllEventsAsync_ShouldReturnAllResults()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _eventRepository = new EventRepository(_context,_mapper);
        // Act
        var result = await _eventRepository.GetAllEventsAsync(1, 2);

        // Assert
        Assert.Equal(_context.Events.Count(), result.Count());
    }

    
    // получение 1 мероприятия
    
    [Fact]
    public async Task GetEventByIdAsync_ShouldReturnEvent_WhenIdIsValid()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _eventRepository = new EventRepository(_context,_mapper);
        // Act
        var result = await _eventRepository.GetEventByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Event", result.Name);
    }

    [Fact]
    public async Task GetEventByIdAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsInvalid()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _eventRepository = new EventRepository(_context,_mapper);
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _eventRepository.GetEventByIdAsync(0));
    }

    [Fact]
    public async Task GetEventByIdAsync_ShouldThrowInvalidOperationException_WhenEventNotFound()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _eventRepository = new EventRepository(_context,_mapper);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _eventRepository.GetEventByIdAsync(999));
    }
    
    
    [Fact]
    public async Task GetEventByNameAsync_ShouldReturnEvent_WhenNameIsValid()
    {
        using var _context = new ApplicationDbContext(_options);
        SeedDatabase(_context);
        var _eventRepository = new EventRepository(_context,_mapper);
        // Act
        var result = await _eventRepository.GetEventByNameAsync("Test Event");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Event", result.Name);
    }

    
    
    [Fact]
    public async Task AddEventAsync_ShouldAddEventToDatabase_WhenNewEventIsValid()
    {
        using var _context = new ApplicationDbContext(_options);
        var _repository = new EventRepository(_context, _mapper);
        SeedDatabase(_context);

        var newEvent = new EventDTO
        {
            Name = "Test Event",
            Description = "Description",
            Location = "Location",
            Category = "Category",
            Date = DateTime.UtcNow,
            MaxParticipants = 100,
            ImageData =new FormFile(new MemoryStream(new byte[2]), 0, 2, "ImageSrc1", "ImageSrc1")
        };

        await _repository.AddEventAsync(newEvent);

        var addedEvent = await _context.Events.FirstOrDefaultAsync(e => e.Name == "Test Event");
        Assert.NotNull(addedEvent);
        Assert.Equal("Test Event", addedEvent.Name);
        Assert.Equal("Description", addedEvent.Description);
        Assert.Equal("Location", addedEvent.Location);
        Assert.Equal("Category", addedEvent.Category);
        Assert.Equal(100, addedEvent.MaxParticipants);
        Assert.NotNull(addedEvent.ImageData);
        Assert.Equal(new byte[2], addedEvent.ImageData);
    }

    
    
    

    [Fact]
    public async Task UpdateEventAsync_ShouldThrowInvalidOperationException_WhenEventNotFound()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateEventAsync(52, new EventDTO
        {
            Name = "Updated Event",
            Description = "Updated Description"
        }));
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldUpdateEvent_WhenEventExists()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем событие в базу данных
        var newEvent = new EventDTO
        {
            Name = "Test Event",
            Description = "Description",
            Location = "Location",
            Category = "Category",
            Date = DateTime.UtcNow,
            MaxParticipants = 100,
            ImageData = new FormFile(new MemoryStream(new byte[2]), 0, 2, "ImageSrc1", "ImageSrc1")
        };
        await repository.AddEventAsync(newEvent);

        // Получаем добавленное событие
        var addedEvent = await context.Events.FirstOrDefaultAsync(e => e.Name == "Test Event");
        Assert.NotNull(addedEvent);

        // Обновляем событие
        var updatedEvent = new EventDTO
        {
            Name = "Updated Event",
            Description = "Updated Description",
            Location = "Updated Location",
            Category = "Updated Category",
            Date = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 200,
            ImageData = new FormFile(new MemoryStream(new byte[2]), 0, 2, "ImageSrc1", "ImageSrc1")
        };
        await repository.UpdateEventAsync(addedEvent.Id, updatedEvent);

        // Проверяем обновленные данные в базе данных
        var updatedEventInDb = await context.Events.FindAsync(addedEvent.Id);
        Assert.NotNull(updatedEventInDb);
        Assert.Equal("Updated Event", updatedEventInDb.Name);
        Assert.Equal("Updated Description", updatedEventInDb.Description);
        Assert.Equal("Updated Location", updatedEventInDb.Location);
        Assert.Equal("Updated Category", updatedEventInDb.Category);
        Assert.Equal(updatedEvent.Date, updatedEventInDb.Date);
        Assert.Equal(updatedEvent.MaxParticipants, updatedEventInDb.MaxParticipants);
        Assert.NotNull(updatedEventInDb.ImageData);
    }
    
    [Fact]
    public async Task DeleteEventAsync_ShouldThrowInvalidOperationException_WhenEventNotFound()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.DeleteEventAsync(15));
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldDeleteEvent_WhenEventExists()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем событие в базу данных
        var newEvent = new EventDTO
        {
            Name = "Test Event",
            Description = "Description",
            Location = "Location",
            Category = "Category",
            Date = DateTime.UtcNow,
            MaxParticipants = 100,
            ImageData = new FormFile(new MemoryStream(new byte[2]), 0, 2, "ImageSrc1", "ImageSrc1")
        };
        await repository.AddEventAsync(newEvent);

        // Получаем добавленное событие
        var addedEvent = await context.Events.FirstOrDefaultAsync(e => e.Name == "Test Event");
        Assert.NotNull(addedEvent);

        // Удаляем событие
        await repository.DeleteEventAsync(addedEvent.Id);
        await context.SaveChangesAsync();

        // Проверяем, что событие успешно удалено из базы данных
        var deletedEvent = await context.Events.FindAsync(addedEvent.Id);
        Assert.Null(deletedEvent);
    }


    [Fact]
    public async Task GetEventsByCriteriaAsync_ShouldReturnFilteredEvents_WhenCriteriaIsProvided()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем несколько событий с разными критериями
        var events = new List<Event>
        {
            new Event { Name = "Event 1", Location = "Location 1", Category = "Category 1" ,Description = "", ImageData = new byte[2]},
            new Event { Name = "Event 2", Location = "Location 1", Category = "Category 2" ,Description = "", ImageData = new byte[2]},
            new Event { Name = "Event 3", Location = "Location 1", Category = "Category 2" ,Description = "", ImageData = new byte[2]}
        };
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Фильтруем события по критериям
        var criteria = new EventCriteria { Location = "Location 1", Category = "Category 2" };
        var filteredEvents = await repository.GetEventsByCriteriaAsync(criteria, 1, 10);

        // Проверяем, что возвращены только отфильтрованные события
        Assert.Equal(2, filteredEvents.Count);
        Assert.All(filteredEvents, e =>
        {
            Assert.Equal("Location 1", e.Location);
            Assert.Equal("Category 2", e.Category);
        });
    }

    [Fact]
    public async Task GetEventsByCriteriaAsync_ShouldPaginateResults()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем несколько событий
        var events = new List<Event>
        {
            new Event {
                Name = "Event 1",
                Location = "Location 1",
                Category = "Category 1",Description = "", ImageData = new byte[2]
                
            },
            new Event { Name = "Event 2", Location = "Location 1", Category = "Category 1",Description = "", ImageData = new byte[2]},
            new Event { Name = "Event 3", Location = "Location 1", Category = "Category 1",Description = "", ImageData = new byte[2] }
        };
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Запрашиваем первую страницу событий
        var criteria = new EventCriteria { Location = "Location 1", Category = "Category 1" };
        var pageSize = 2;
        var page1Events = await repository.GetEventsByCriteriaAsync(criteria, 1, pageSize);

        // Проверяем, что вернулось правильное количество событий для первой страницы
        Assert.Equal(pageSize, page1Events.Count);

        // Запрашиваем вторую страницу событий
        var page2Events = await repository.GetEventsByCriteriaAsync(criteria, 2, pageSize);

        // Проверяем, что вернулось правильное количество событий для второй страницы
        Assert.Equal(1, page2Events.Count);
    }
    
    
    [Fact]
    public async Task SearchEvents_ShouldReturnEventsByDate_WhenDateIsSpecified()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем несколько событий с разными датами
        var events = new List<Event>
        {
            new Event
            {
                Name = "Event 1", Date = DateTime.UtcNow.Date.AddDays(-1)
                ,Category = "", Location = "", Description = "", ImageData = new byte[2]
            },
            new Event { Name = "Event 2", Date = DateTime.UtcNow.Date ,Category = "", Location = "", Description = "", ImageData = new byte[2]},
            new Event { Name = "Event 3", Date = DateTime.UtcNow.Date.AddDays(1),Category = "", Location = "", Description = "", ImageData = new byte[2] }
        };
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Запрашиваем события по дате
        var searchModel = new SearchDTO { Date = DateTime.UtcNow.Date };
        var pageSize = 10;
        var searchedEvents = await repository.SearchEvents(searchModel, 1, pageSize);

        // Проверяем, что вернулись только события с указанной датой
        Assert.Single(searchedEvents); // Предполагаем, что будет одно событие на указанную дату
        Assert.Equal("Event 2", searchedEvents[0].Name);
    }

    [Fact]
    public async Task SearchEvents_ShouldReturnEventsByName_WhenNameIsSpecified()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем событие с указанным именем
        var eventName = "Test Event1";
        var events = new List<Event>
        {
            new Event { Name = eventName, Date = DateTime.UtcNow.Date,Category = "", Location = "", Description = "", ImageData = new byte[2] }
        };
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Запрашиваем событие по имени
        var searchModel = new SearchDTO { Name = eventName };
        var pageSize = 10;
        var searchedEvents = await repository.SearchEvents(searchModel, 1, pageSize);

        // Проверяем, что вернулось событие с указанным именем
        Assert.Single(searchedEvents);
        Assert.Equal(eventName, searchedEvents[0].Name);
    }

    [Fact]
    public async Task SearchEvents_ShouldPaginateResults()
    {
        using var context = new ApplicationDbContext(_options);
        var repository = new EventRepository(context, _mapper);
        SeedDatabase(context);

        // Добавляем несколько событий
        var events = new List<Event>
        {
            new Event { Name = "Event 1", Date = DateTime.UtcNow.Date,Category = "", Location = "", Description = "", ImageData = new byte[2] },
            new Event { Name = "Event 2", Date = DateTime.UtcNow.Date,Category = "", Location = "", Description = "", ImageData = new byte[2] },
            new Event { Name = "Event 3", Date = DateTime.UtcNow.Date,Category = "", Location = "", Description = "", ImageData = new byte[2] }
        };
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Запрашиваем первую страницу событий
        var searchModel = new SearchDTO
        {
            Name = "Event"
        };
        var pageSize = 2;
        var page1Events = await repository.SearchEvents(searchModel, 1, pageSize);

        // Проверяем, что вернулось правильное количество событий для первой страницы
        Assert.Equal(pageSize, page1Events.Count);

        // Запрашиваем вторую страницу событий
        var page2Events = await repository.SearchEvents(searchModel, 2, pageSize);

        // Проверяем, что вернулось правильное количество событий для второй страницы
        Assert.Equal(2, page2Events.Count);
    }
    
    
}