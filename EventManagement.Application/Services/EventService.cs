using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Repositories;
namespace EventManagement.Application;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private IEventService _eventServiceImplementation;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        var events = _eventRepository.GetAllEventsAsync();
        if(events == null)
        {
            throw new Exception("No events found");
        }
        return events;
    }
    
    public Task<Event> GetEventByIdAsync(int id)
    {
        var eventById = _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new Exception("Event not found");
        }
        return eventById;
    }
    
    public Task<Event> GetEventByNameAsync(string name)
    {
        var eventByName = _eventRepository.GetEventByNameAsync(name);
        if(eventByName == null)
        {
            throw new Exception("Event not found");
        }
        return eventByName;
    }
    
    public Task AddEventAsync(Event newEvent)
    {
        if(newEvent == null)
        {
            throw new Exception("Event is null");
        }
        return _eventRepository.AddEventAsync(newEvent);
    }
    
    public Task UpdateEventAsync(Event updatedEvent)
    {
        if(updatedEvent == null)
        {
            throw new Exception("Event is null");
        }
        return _eventRepository.UpdateEventAsync(updatedEvent);
    }
    
    public Task DeleteEventAsync(int id)
    {
        var eventById = _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new Exception("Event not found");
        }
        return _eventRepository.DeleteEventAsync(id);
    }
    
    public Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria)
    {
        if(criteria == null)
        {
            throw new Exception("Criteria is null");
        }
        return _eventRepository.GetEventsByCriteriaAsync(criteria);
    }
}