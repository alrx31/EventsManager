using EventManagement.Domain.Entities;

namespace EventManagement.Infrastructure.Repositories;

public interface IParticipantRepository
{
    Task RegisterParticipantToEventAsync(int eventId, int participantId);
    Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
    Task<Participant> GetParticipantByIdAsync(int id);
    Task CancelRegistrationAsync(int eventId, int participantId);
    Task SendEmailToParticipantAsync(int eventId, int participantId, string message);
    
}