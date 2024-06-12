using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
namespace EventManagement.Application;
using Microsoft.AspNetCore.Hosting;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public EventService(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
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
    
    public async Task AddEventAsync(EventDTO newEvent)
    {
        if(newEvent == null)
        {
            throw new Exception("Event is null");
        }
        // check all required fields
        if(string.IsNullOrEmpty(newEvent.Name) ||
           string.IsNullOrEmpty(newEvent.Description) ||
           newEvent.Date == null ||
           newEvent.MaxParticipants == null
           )
        {
            throw new Exception("Required fields are empty");
        }

        await _eventRepository.AddEventAsync(newEvent);
        await _unitOfWork.CompleteAsync();

    }
    
    public async Task UpdateEventAsync(int eventId,EventDTO updatedEvent)
    {
        if(updatedEvent == null || eventId < 1)
        {
            throw new Exception("Event is null");
        }
        await _eventRepository.UpdateEventAsync(eventId,updatedEvent);
        await _unitOfWork.CompleteAsync();
    }
    
    public async Task DeleteEventAsync(int id)
    {
        var eventById = await _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new Exception("Event not found");
        }
        await _eventRepository.DeleteEventAsync(id);
        await _unitOfWork.CompleteAsync();
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