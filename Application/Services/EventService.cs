using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
namespace EventManagement.Application;
using Microsoft.AspNetCore.Hosting;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        var events = await _eventRepository.GetAllEventsAsync();
        if(events == null)
        {
            throw new Exception("No events found");
        }
        return events;
    }
    
    public async Task<Event> GetEventByIdAsync(int id)
    {
        var eventById = await _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new Exception("Event not found");
        }
        return eventById;
    }
    
    public async Task<Event> GetEventByNameAsync(string name)
    {
        var eventByName = await _eventRepository.GetEventByNameAsync(name);
        if(eventByName == null)
        {
            throw new Exception("Event not found");
        }
        return eventByName;
    }
    
    public async Task<Task> AddEventAsync(Event newEvent)
    {
        if(newEvent == null)
        {
            throw new Exception("Event is null");
        }
        return _eventRepository.AddEventAsync(newEvent);
    }
    
    public async Task<Task> UpdateEventAsync(Event updatedEvent)
    {
        if(updatedEvent == null)
        {
            throw new Exception("Event is null");
        }
        return _eventRepository.UpdateEventAsync(updatedEvent);
    }
    
    public async Task<Task> DeleteEventAsync(int id)
    {
        var eventById = await _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new Exception("Event not found");
        }
        return _eventRepository.DeleteEventAsync(id);
    }
    
    public async Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria)
    {
        if(criteria == null)
        {
            throw new Exception("Criteria is null");
        }
        return await _eventRepository.GetEventsByCriteriaAsync(criteria);
    }
    
    
}