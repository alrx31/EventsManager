using EventManagement.Application.Models;

namespace EventManagement.Domain.Entities

{ 
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public int MaxParticipants { get; set; }
        //add image 
        
        
        public List<Participant> Participants { get; set; }
        public List<EventParticipant> EventParticipants { get; set; }
    }
}