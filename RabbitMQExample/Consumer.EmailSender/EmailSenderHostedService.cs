using Consumer.EmailSender.Models;
using EmailService.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;
using SystemFacade;

namespace Consumer.EmailSender;

public class EmailSenderHostedService : BaseHostedService
{
    private readonly IDbLogger<SendEmailConsumer> _dbLogger;
    private readonly IProgress<string> _progress;
    private readonly ISmtpService _emailsService;
    private readonly BaseExchange<CertificateEvent> _exchange;
    private readonly EmailSenderSettings _settings;

    public EmailSenderHostedService(IDbLogger<SendEmailConsumer> dbLogger, 
        IProgress<string> progress, 
        ISmtpService emailsService,
        BaseExchange<CertificateEvent> exchange,
        IOptions<EmailSenderSettings> settings) : base(progress)
    {
        _dbLogger = dbLogger;
        _progress = progress;
        _emailsService = emailsService;
        _exchange = exchange;
        _settings = settings.Value;
    }
    protected override async Task DoWork(CancellationToken token)
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

    public override void Dispose()
    {
        base.Dispose();
        _exchange.Dispose();
    }
}