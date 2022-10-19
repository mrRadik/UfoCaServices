using System.Net.Mail;
using System.Text;
using Consumer.EmailSender.Models;
using EmailService.Interfaces;
using EmailService.Models;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQBase;
using Services.Interfaces;

namespace Consumer.EmailSender;

public class SendEmailConsumer : RabbitMqConsumerBase
{
    private readonly IProgress<string> _progress;
    private readonly ISmtpService _emailService;
    private readonly IDbLogger<SendEmailConsumer> _dbLogger;

    public SendEmailConsumer(
        IProgress<string> progress, 
        ISmtpService emailService, 
        IDbLogger<SendEmailConsumer> dbLogger,
        IBaseExchange exchange,
        CancellationToken token) : base(progress, exchange,token)
    {
        _progress = progress;
        _emailService = emailService;
        _dbLogger = dbLogger;
    }

    protected override async void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        base.OnNewMessageReceived(sender, e);
        var settings = ApplicationSettings.GetInstance();
        var message = Encoding.Default.GetString(e.Body.ToArray());
        var certificate = JsonConvert.DeserializeObject<CertificateModel>(message);
        
        var mail = new MailModel
        {
            From = new MailAddress(settings.EmailSenderSettings.Addresses.FromMail,
                settings.EmailSenderSettings.Addresses.FromDisplayName),
            To = new MailAddress(settings.EmailSenderSettings.Addresses.ToEmail),
            Body = string.Format(settings.EmailSenderSettings.Mail.Body, certificate.Subject),
            Subject = settings.EmailSenderSettings.Mail.Subject
        };

        try
        {
            await _emailService.SendEmailAsync(mail);
            _progress.Report("Email sent");
            if (!settings.EmailSenderSettings.AutoAck)
            {
                Channel.BasicAck(e.DeliveryTag, false);
            }
        }
        catch (Exception ex)
        {
            _progress.Report("Email not delivered. For more information see logs");
            await _dbLogger.LogError(ex.Message);
        }
    }
}