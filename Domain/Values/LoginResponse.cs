namespace EventManagement.Application.Models;

public class LoginResponse
{
    public bool IsLoggedIn { get; set; } = false;
    public string JwtToken { get; set; }    
    public string RefreshToken { get; set; }
}