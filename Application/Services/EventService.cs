using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Middlewares;
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
    
    public async Task<IEnumerable<EventRequest>> GetAllEventsAsync(int page,int pageSize)
    {
        if(page < 1 || pageSize < 1)
        {
            throw new ValidationException("Invalid page or pageSize");
        }
        
        var events = await _eventRepository.GetAllEventsAsync(page,pageSize);
        if(events == null)
        {
            throw new NotFoundException("No events found");
        }
        return events;
    }
    
    public async Task<Event> GetEventByIdAsync(int id)
    {
        if (id < 1)
        {
            throw new ValidationException("Invalid Event Id");
        }
        var eventById = await _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new NotFoundException("Event not found");
        }
        return eventById;
    }
    
    public async Task<EventRequest> GetEventByIdAsyncRequest(int id)
    {
        if (id <= 0)
        {
            throw new ValidationException("Invalid event ID");
        }
        var eventById = await _eventRepository.GetEventByIdAsyncRequest(id);
        if(eventById == null)
        {
            throw new NotFoundException("Event not found");
        }
        return eventById;
    }
    
    public async Task<Event> GetEventByNameAsync(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            throw new ValidationException("Name is null");
        }
        var eventByName = await _eventRepository.GetEventByNameAsync(name);
        if(eventByName == null)
        {
            throw new NotFoundException("Event not found");
        }
        return eventByName;
    }
    
    public async Task AddEventAsync(EventDTO newEvent)
    {        
        await _eventRepository.AddEventAsync(newEvent);
        await _unitOfWork.CompleteAsync();

    }
    
    public async Task UpdateEventAsync(int eventId,EventDTO updatedEvent)
    {
        await _eventRepository.GetEventByIdAsync(eventId);
        await _eventRepository.UpdateEventAsync(eventId,updatedEvent);
        await _unitOfWork.CompleteAsync();
    }
    
    public async Task DeleteEventAsync(int id)
    {
        var eventById = await _eventRepository.GetEventByIdAsync(id);
        if(eventById == null)
        {
            throw new NotFoundException("Event not found");
        }
        await _eventRepository.DeleteEventAsync(id);
        await _unitOfWork.CompleteAsync();
    }
    

    public async Task<List<EventRequest>> GetEventsByCriteriaAsync(EventCriteria criteria,int page,int pageSize)
    {
        if(criteria == null)
        {
            throw new ValidationException("Criteria is null");
        }
        if(page < 1 || pageSize < 1)
        {
            throw new ValidationException("Invalid page or pageSize");
        }
        // если параметр пустой, он не учитывается — допускаем фильтр по одному или ни по одному параметру
        return await _eventRepository.GetEventsByCriteriaAsync(criteria, page, pageSize);
    }

    public async Task<List<EventRequest>> getEventsByUserId(int id)
    {
        if (id < 1) throw new ValidationException("Invalid Id");
        return await _eventRepository.getEventsByUserId(id);
    }

    public async Task<List<EventRequest>> SearchEvents(SearchDTO model,int page, int pageSize)
    {
        if(model == null)
        {
            throw new ValidationException("Model is null");
        }
        if(page < 1 || pageSize < 1)
        {
            throw new ValidationException("Invalid page or pageSize");
        }
        
        if(model.Date == new DateTime() && string.IsNullOrEmpty(model.Name))
        {
            throw new ValidationException("Date or Name is required");
        }
        
        return await _eventRepository.SearchEvents(model,page,pageSize);
        
    }

    public async Task<int> GetCountEvents()
    {
        return await _eventRepository.GetCountEvents();
    }

    public async Task<int> GetCountEventsSearch(SearchDTO model)
    {
        return await _eventRepository.GetCountEventsSearch(model);
    }
    public async Task<int> GetCountEventsFilter(EventCriteria model)
    {
        return await _eventRepository.GetCountEventsFilter(model);
    }
}