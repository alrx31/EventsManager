using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Infrastructure.Repositories;

public class ParticipantService:IParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _key;

    public ParticipantService(IParticipantRepository participantRepository, IConfiguration config, IUnitOfWork unitOfWork)
    {
        _participantRepository = participantRepository;
        _unitOfWork = unitOfWork;
        _key = config["Jwt:Key"];
    }

    public async Task RegisterParticipantAsync(ParticipantRegisterDTO user)
    {
        // check all fields
        if (string.IsNullOrEmpty(user.FirstName)) throw new Exception("Invalid First Name");
        if (string.IsNullOrEmpty(user.LastName)) throw new Exception("Invalid Last Name");
        if (user.BirthDate == null) throw new Exception("Invalid Birth Date");
        if (user.RegistrationDate == null) throw new Exception("Invalid Registration Date");
        if (string.IsNullOrEmpty(user.Email)) throw new Exception("Invalid Email");
        if (string.IsNullOrEmpty(user.Password)) throw new Exception("Invalid Password");
        await _participantRepository.RegisterParticipantAsync(user);
        await _participantRepository.AddRefreshTokenField(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<string> Login(LoginModel model)
    {
        var user = await _participantRepository.LoginAsync(model);
        if(user == null) throw new Exception("Invalid Credentials");
        var token = GenerateJwtToken(user);
        return token;
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
    
    private string GenerateJwtToken(Participant user){
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    
}