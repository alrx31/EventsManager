using System;
using EventManagement.Application.Models;

namespace EventManagement.Domain.Entities
{

    public class Participant
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Email { get; set; }
        public List<EventParticipant> EventParticipants { get; set; }
        
        public string Password { get; set; }
        
        
        public List<ExtendedIdentityUser> IdentityUsers { get; set; }
    }
}