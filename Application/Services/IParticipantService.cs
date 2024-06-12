using EventManagement.Application.Models;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Services
{
    public interface IParticipantService
    {
        // main
        Task RegisterParticipantToEventAsync(int eventId, int participantId);
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
        Task<Participant> GetParticipantByIdAsync(int id);
        Task CancelRegistrationAsync(int eventId, int participantId);
        Task SendEmailToParticipantAsync(int eventId, int participantId, string message);
        
        
        // to register
        Task RegisterParticipantAsync(ParticipantRegisterDTO user);
        Task<string> Login(LoginModel model);
    }
}
