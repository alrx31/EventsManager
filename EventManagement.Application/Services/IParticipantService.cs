using EventManagement.Domain.Entities;

namespace EventManagement.Application.Services
{
    public interface IParticipantService
    {
        // main
        Task RegisterParticipantToEventAsync(int eventId, Participant participant);
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
        Task<Participant> GetParticipantByIdAsync(int id);
        Task CancelRegistrationAsync(int eventId, int participantId);
        Task SendEmailToParticipantAsync(int eventId, int participantId, string message);
    }
}