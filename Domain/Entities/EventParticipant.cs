using EventManagement.Domain.Entities;

namespace EventManagement.Application.Models;

public class EventParticipant
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; }
}