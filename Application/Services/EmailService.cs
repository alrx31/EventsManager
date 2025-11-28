using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EventManagement.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUser = _configuration["Email:SmtpUser"] ?? "";
        var smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
        var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
        var fromName = _configuration["Email:FromName"] ?? "Events Manager";
        
        if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
        {
            // Если настройки email не заданы, просто логируем
            Console.WriteLine($"[EMAIL] To: {to}, Subject: {subject}, Body: {body}");
            return;
        }
        
        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPassword),
            EnableSsl = true
        };
        
        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);
        
        await client.SendMailAsync(mailMessage);
    }
    
    public async Task SendPasswordResetEmailAsync(string to, string newPassword)
    {
        var subject = "Восстановление пароля - Events Manager";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2 style='color: #007bff;'>Восстановление пароля</h2>
                <p>Вы запросили восстановление пароля для вашего аккаунта в системе Events Manager.</p>
                <p>Ваш новый пароль: <strong style='font-size: 18px; color: #333;'>{newPassword}</strong></p>
                <p style='color: #666; font-size: 14px;'>
                    Рекомендуем сменить пароль после входа в систему.
                </p>
                <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'/>
                <p style='color: #999; font-size: 12px;'>
                    Если вы не запрашивали восстановление пароля, проигнорируйте это письмо.
                </p>
            </body>
            </html>";
        
        await SendEmailAsync(to, subject, body);
    }
}
