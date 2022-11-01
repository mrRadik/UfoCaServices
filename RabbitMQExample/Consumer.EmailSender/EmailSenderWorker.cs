using Consumer.EmailSender.Models;
using EmailService.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;

namespace Consumer.EmailSender;

public interface IEmailSenderWorker : IBaseWorker
{
}

public class EmailSenderWorker : IEmailSenderWorker
{
    private readonly IDbLogger<SendEmailConsumer> _dbLogger;
    private readonly IProgress<string> _progress;
    private readonly ISmtpService _emailsService;
    private readonly BaseExchange<CertificateEvent> _exchange;
    private readonly EmailSenderSettings _settings;

    public EmailSenderWorker(IDbLogger<SendEmailConsumer> dbLogger, 
        IProgress<string> progress, 
        ISmtpService emailsService,
        BaseExchange<CertificateEvent> exchange,
        IOptions<EmailSenderSettings> settings)
    {
        _dbLogger = dbLogger;
        _progress = progress;
        _emailsService = emailsService;
        _exchange = exchange;
        _settings = settings.Value;
    }
    public async Task Start(CancellationToken token)
    {
        await Task.Run(() =>
        {
            var consumer = new SendEmailConsumer(_progress, _emailsService, _dbLogger, _exchange, _settings, token);
            try
            {
                consumer.SubscribeAndReceive();
            }
            catch (Exception ex)
            {
                _progress.Report("Can't connect to Rabbit MQ queue. See logs");
                _dbLogger.LogError(ex.Message);
            }
        }, token);

    }
}