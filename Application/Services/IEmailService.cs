using System.Threading.Tasks;

namespace EventManagement.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(string to, string newPassword);
}
