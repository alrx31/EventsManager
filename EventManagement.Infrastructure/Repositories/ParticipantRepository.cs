using EventManagement.Application.Models;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly ApplicationDbContext _context;
    public ParticipantRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task RegisterParticipantToEventAsync(int eventId, Participant participant)
    {
        _context.EventParticipants.Add(new EventParticipant
        {
            EventId = eventId,
            ParticipantId = participant.Id
        });
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId)
    {
        return await _context.EventParticipants
            .Where(ep => ep.EventId == eventId)
            .Select(ep => ep.Participant)
            .ToListAsync();
    }

    public async Task<Participant> GetParticipantByIdAsync(int id)
    {
        return await _context.Participants.FindAsync(id);
    }

    public async Task CancelRegistrationAsync(int eventId, int participantId)
    {
        var eventParticipant = await _context.EventParticipants
            .Where(ep => ep.EventId == eventId && ep.ParticipantId == participantId)
            .FirstOrDefaultAsync();
        if (eventParticipant != null)
        {
            _context.EventParticipants.Remove(eventParticipant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SendEmailToParticipantAsync(int eventId, int participantId, string message)
    {
        ///
    }
}

