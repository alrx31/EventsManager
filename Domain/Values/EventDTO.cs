using Microsoft.AspNetCore.Http;

namespace EventManagement.Application.Models;

public class EventDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxParticipants { get; set; }
    public IFormFile ImageData { get; set; }
}