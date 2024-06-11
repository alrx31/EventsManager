using EventManagement.Application.Services;
using EventManagement.Domain.Entities;

namespace EventManagement.Infrastructure.Repositories;

public class ParticipantService:IParticipantService
{
    private readonly IParticipantRepository _participantRepository;

    public ParticipantService(IParticipantRepository participantRepository)
    {
        _participantRepository = participantRepository;
    }
    
    public Task RegisterParticipantToEventAsync(int eventId, int participantId)
    {
        if (eventId < 1) throw new Exception("Invalid Event Id");
        if (participantId < 1) throw new Exception("Invalid Participant");
        return _participantRepository.RegisterParticipantToEventAsync(eventId, participantId);
    }

    public Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId)
    {
        var participants = _participantRepository.GetParticipantsByEventIdAsync(eventId);
        if(participants == null) throw new Exception("No Participants Found");
        return participants;
    }

    public Task<Participant> GetParticipantByIdAsync(int id)
    {
        var participant = _participantRepository.GetParticipantByIdAsync(id);
        if(participant == null) throw new Exception("Participant Not Found");
        return participant;
    }

    public Task CancelRegistrationAsync(int eventId, int participantId)
    {
        if(eventId < 1) throw new Exception("Invalid Event Id");
        if(participantId < 1) throw new Exception("Invalid Participant Id");
        return _participantRepository.CancelRegistrationAsync(eventId, participantId);
    }

    public Task SendEmailToParticipantAsync(int eventId, int participantId, string message)
    {
        if(eventId < 1) throw new Exception("Invalid Event Id");
        if(participantId < 1) throw new Exception("Invalid Participant Id");
        if(string.IsNullOrEmpty(message)) throw new Exception("Invalid Message");
        return _participantRepository.SendEmailToParticipantAsync(eventId, participantId, message);
    }
    
    
    
}