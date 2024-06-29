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
        Task<IEnumerable<EventRequest>> GetAllEventsAsync(int page);
        Task<Event> GetEventByIdAsync(int id);
        Task<Event> GetEventByNameAsync(string name);
        Task AddEventAsync(EventDTO newEvent);
        Task UpdateEventAsync(int eventId,EventDTO updatedEvent);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria);
        
        // another
        /*Task AddParticipantToEventAsync(int eventId, Participant participant);
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
        Task<Participant> GetParticipantByIdAsync(int id);
        Task RemoveParticipantFromEventAsync(int eventId, int participantId);*/

    }
}