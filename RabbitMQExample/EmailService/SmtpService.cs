using System.Net.Mail;
using EmailService.Interfaces;
using EmailService.Models;

namespace EmailService;

public class SmtpService : ISmtpService
{
    private readonly SmtpClient _client;
    
    public SmtpService(SmtpSettings settings)
    {
        _client = EmailClient.GetInstance(settings).Client;
    }
    
    public async Task SendEmailAsync(MailModel mailDto)
    {
        var mail = CreateMessage(mailDto);
        await _client.SendMailAsync(mail);
    }
    
    private static MailMessage CreateMessage(MailModel mailDto)
    {
        return new MailMessage(mailDto.From, mailDto.To)
        {
            Subject = mailDto.Subject,
            Body = mailDto.Body
        };
    }
}