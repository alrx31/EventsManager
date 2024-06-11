using EventManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ParticipantsController:ControllerBase
{
    private readonly IParticipantService _participantService;
    
    
    public ParticipantsController(IParticipantService participantService)
    {
        _participantService = participantService;
    }
    
    //1. Регистрация участия пользователя в событии;
    [HttpPut("{eventId}/register/{participantId}")]
    public async Task<IActionResult> RegisterParticipantToEventAsync(int eventId, int participantId)
    {
        await _participantService.RegisterParticipantToEventAsync(eventId, participantId);
        return Ok();
    }
    
    
    //2. Получение списка участников события;
    [HttpGet("{eventId}/participants")]
    public async Task<IActionResult> GetParticipantsByEventIdAsync(int eventId)
    {
        var participants = await _participantService.GetParticipantsByEventIdAsync(eventId);
        if(participants == null)
        {
            return NotFound();
        }
        return Ok(participants);
    }
    //3. Получение определенного участника по его Id;
    [HttpGet("{participantId}")]
    public async Task<IActionResult> GetParticipantByIdAsync(int participantId)
    {
        var participant = await _participantService.GetParticipantByIdAsync(participantId);
        if(participant == null)
        {
            return NotFound();
        }
        return Ok(participant);
    }
    //4. Отмена участия пользователя в событии;
    [HttpDelete("{eventId}/cancel/{participantId}")]
    public async Task<IActionResult> CancelRegistrationAsync(int eventId, int participantId)
    {
        await _participantService.CancelRegistrationAsync(eventId, participantId);
        return Ok();
    }
    
    
}