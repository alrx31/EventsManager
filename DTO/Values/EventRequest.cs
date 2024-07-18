using System;

namespace EventManagement.Application.Models;

public class EventRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxParticipants { get; set; }
    public string ImageSrc { get; set; }
}