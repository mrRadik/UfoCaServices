using EmailService.Models;
using RabbitMQBase.Models;

namespace Consumer.EmailSender.Models;

public class EmailSenderSettings : ConsumerSettings
{
    public Addresses Addresses { get; set; } = default!;
    public SmtpSettings Smtp { get; set; }= default!;
    
    public Mail Mail { get; set; }= default!;
}

public class Addresses
{
    public string FromMail { get; set; }= default!;
    public string FromDisplayName { get; set; }= default!;
    public string ToEmail { get; set; }= default!;
}

public class Mail
{
    public string Subject { get; set; }= default!;
    public string Body { get; set; }= default!;
}
