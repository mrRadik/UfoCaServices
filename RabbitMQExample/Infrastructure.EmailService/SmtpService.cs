using System.Net;
using System.Net.Mail;
using EmailService.Interfaces;
using EmailService.Models;

namespace EmailService;

public class SmtpService : ISmtpService
{
    private readonly SmtpClient _client;
    
    public SmtpService(SmtpSettings settings)
    {
        _client = new SmtpClient
        {
            Credentials = new NetworkCredential(settings.UserName, settings.Password),
            Host = settings.Host,
            Port = settings.Port, 
            EnableSsl = settings.Ssl,
            DeliveryMethod = (SmtpDeliveryMethod)settings.SmtpDeliveryMethod,
            PickupDirectoryLocation = settings.PickupDirectoryLocation
        };
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