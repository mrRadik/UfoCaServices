using EmailService.Models;

namespace EmailService.Interfaces;

public interface ISmtpService
{
    Task SendEmailAsync(MailModel mailDto);
}