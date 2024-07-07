using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace EventManagement.Infrastructure.Repositories
{
    public interface IEventRepository
    {
        // main
        Task<IEnumerable<EventRequest>> GetAllEventsAsync(int page,int pageSize);
        Task<Event> GetEventByIdAsync(int id);
        Task<EventRequest> GetEventByIdAsyncRequest(int id);
        Task<Event> GetEventByNameAsync(string name);
        Task AddEventAsync(EventDTO newEvent);
        Task UpdateEventAsync(int eventId,EventDTO updatedEvent);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria);
        Task<List<EventRequest>> getEventsByUserId(int id);
        Task<List<EventRequest>> SearchEvents(SearchDTO model,int page,int pageSize);
        Task<int> GetCountEvents();

    }
}