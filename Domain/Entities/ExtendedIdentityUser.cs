using EventManagement.Domain.Entities;

namespace EventManagement.Application.Models;

public class ExtendedIdentityUser
{
    public int Id { get; set; }
    
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; }
    
    public string Email { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}