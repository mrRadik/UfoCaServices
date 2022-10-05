using System.Net.Mail;

namespace EmailService.Models;

public class MailModel
{
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
    public MailAddress From { get; set; } = default!;
    public MailAddress To { get; set; } = default!;
}