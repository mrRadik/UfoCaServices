using System.Net.Mail;
using Consumer.EmailSender.Models;
using EmailService.Interfaces;
using EmailService.Models;
using RabbitMQ.Client.Events;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;

namespace Consumer.EmailSender;

public class SendEmailConsumer : RabbitMqConsumerBase<CertificateEvent>
{
    private readonly IProgress<string> _progress;
    private readonly ISmtpService _emailService;
    private readonly IDbLogger<SendEmailConsumer> _dbLogger;
    private readonly EmailSenderSettings _settings;

    public SendEmailConsumer(
        IProgress<string> progress, 
        ISmtpService emailService, 
        IDbLogger<SendEmailConsumer> dbLogger,
        BaseExchange<CertificateEvent> exchange,
        EmailSenderSettings settings,
        CancellationToken token) : base(progress, exchange, settings,token)
    {
        _progress = progress;
        _emailService = emailService;
        _dbLogger = dbLogger;
        _settings = settings;
    }

    protected override async void HandleMessage(CertificateEvent cert, BasicDeliverEventArgs e)
    {
        var mail = new MailModel
        {
            From = new MailAddress(_settings.Addresses.FromMail,
                _settings.Addresses.FromDisplayName),
            To = new MailAddress(_settings.Addresses.ToEmail),
            Body = string.Format(_settings.Mail.Body, cert.Subject),
            Subject = _settings.Mail.Subject
        };

        await _emailService.SendEmailAsync(mail);
        _progress.Report($"Email from {mail.From.Address} to {mail.To.Address} sent. Body: {mail.Body}");
    }

    protected override async void HandleError(Exception exception)
    {
        _progress.Report("Something went wrong. See logs");
        await _dbLogger.LogError(exception.Message);
    }
}