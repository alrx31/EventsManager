using System;

namespace EventManagement.Application.Models;

public class ParticipantRegisterDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Email { get; set; }
    
    public string Password { get; set; }
}