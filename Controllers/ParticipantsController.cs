using System.Threading.Tasks;
using EventManagement.Application.Models;
using EventManagement.Application.Services;
using EventManagement.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ParticipantsController:ControllerBase
{
    private readonly IParticipantService _participantService;
    private readonly IAuthService _authService;
    
    public ParticipantsController(IParticipantService participantService, IAuthService authService)
    {
        _participantService = participantService;
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterParticipantAsync([FromBody] ParticipantRegisterDTO participantRegisterDTO)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        await _participantService.RegisterParticipantAsync(participantRegisterDTO);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var loginRes = await _authService.Login(model);
        if (loginRes.IsLoggedIn) return Ok(loginRes);
        return Unauthorized();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
    {
        var loginRes = await _authService.RefreshToken(model);
        if (loginRes.IsLoggedIn) return Ok(loginRes);
        return Unauthorized();
    }

    
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutModel model)
    {
        await _authService.Logout(model);
        return Ok();
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequestDTO model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _authService.ResetPasswordAsync(model.Email);
        return Ok(result);
    }

    //1. Регистрация участия пользователя в событии;+
    [HttpPut("{eventId}/register/{participantId}")]
    
    public async Task<IActionResult> RegisterParticipantToEventAsync(int eventId, int participantId)
    {
        await _participantService.RegisterParticipantToEventAsync(eventId, participantId);
        return Ok();
    }
    //2. Получение списка участников события;+
    [HttpGet("{eventId}/participants")]
    
    public async Task<IActionResult> GetParticipantsByEventIdAsync(int eventId)
    {
        var participants = await _participantService.GetParticipantsByEventIdAsync(eventId);
        if(participants == null)
        {
            throw new NotFoundException();
        }
        return Ok(participants);
    }
    //3. Получение определенного участника по его Id;  +
    [HttpGet("{participantId}")]
    public async Task<IActionResult> GetParticipantByIdAsync(int participantId)
    {
        var participant = await _participantService.GetParticipantByIdAsync(participantId);
        if(participant == null)
        {
            throw new NotFoundException();
        }
        return Ok(participant);
    }
    //4. Отмена участия пользователя в событии; +
    [HttpDelete("{eventId}/cancel/{participantId}")]
    
    public async Task<IActionResult> CancelRegistrationAsync(int eventId, int participantId)
    {
        await _participantService.CancelRegistrationAsync(eventId, participantId);
        return Ok();
    }
    
    
}