using EmailService.Models;

namespace Consumer.EmailSender.Models;

internal class EmailSenderSettings
{
    public Addresses Addresses { get; set; } = default!;
    public SmtpSettings Smtp { get; set; }= default!;
    public Mail Mail { get; set; }= default!;
}

internal class Addresses
{
    public string FromMail { get; set; }= default!;
    public string FromDisplayName { get; set; }= default!;
    public string ToEmail { get; set; }= default!;
}

internal class Mail
{
    public string Subject { get; set; }= default!;
    public string Body { get; set; }= default!;
}
