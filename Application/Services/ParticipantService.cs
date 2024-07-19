using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Middlewares;
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
        await _participantRepository.RegisterParticipantAsync(user);
        await _unitOfWork.CompleteAsync();
        await _participantRepository.AddRefreshTokenField(user);
        await _unitOfWork.CompleteAsync();
        
    }


    public Task RegisterParticipantToEventAsync(int eventId, int participantId)
    {
        if (eventId < 1) throw new ValidationException("Invalid Event Id");
        if (participantId < 1) throw new ValidationException("Invalid Participant");
        return _participantRepository.RegisterParticipantToEventAsync(eventId, participantId);
    }

    public Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId)
    {
        var participants = _participantRepository.GetParticipantsByEventIdAsync(eventId);
        if(participants == null) throw new NotFoundException("No Participants Found");
        return participants;
    }

    public Task<Participant> GetParticipantByIdAsync(int id)
    {
        var participant = _participantRepository.GetParticipantByIdAsync(id);
        if(participant == null) throw new NotFoundException("Participant Not Found");
        return participant;
    }

    public Task CancelRegistrationAsync(int eventId, int participantId)
    {
        if(eventId < 1) throw new ValidationException("Invalid Event Id");
        if(participantId < 1) throw new ValidationException("Invalid Participant Id");
        return _participantRepository.CancelRegistrationAsync(eventId, participantId);
    }

    public Task SendEmailToParticipantAsync(int eventId, int participantId, string message)
    {
        if(eventId < 1) throw new ValidationException("Invalid Event Id");
        if(participantId < 1) throw new ValidationException("Invalid Participant Id");
        if(string.IsNullOrEmpty(message)) throw new ValidationException("Invalid Message");
        return _participantRepository.SendEmailToParticipantAsync(eventId, participantId, message);
    }
    
}