using EventManagement.Application.Models;
using EventManagement.Domain.Entities;

namespace EventManagement.Infrastructure.Repositories
{
    public interface IEventRepository
    {
        // main
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task<Event> GetEventByNameAsync(string name);
        Task AddEventAsync(Event newEvent);
        Task UpdateEventAsync(Event updatedEvent);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<Event>> GetEventsByCriteriaAsync(EventCriteria criteria);
        
        
        // another
        /*Task AddParticipantToEventAsync(int eventId, Participant participant);
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
        Task<Participant> GetParticipantByIdAsync(int id);
        Task RemoveParticipantFromEventAsync(int eventId, int participantId);*/

    }
}