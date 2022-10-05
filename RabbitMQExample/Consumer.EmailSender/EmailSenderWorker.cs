using BusinessFacade;
using BusinessFacade.Services;
using EmailService.Interfaces;

namespace Consumer.EmailSender;

public interface IEmailSenderWorker : IBaseWorker
{
}

public class EmailSenderWorker : IEmailSenderWorker
{
    private readonly IDbLogger<SendEmailConsumer> _dbLogger;
    private readonly IProgress<string> _progress;
    private readonly ISmtpService _emailsService;

    public EmailSenderWorker(IDbLogger<SendEmailConsumer> dbLogger, IProgress<string> progress, ISmtpService emailsService)
    {
        _dbLogger = dbLogger;
        _progress = progress;
        _emailsService = emailsService;
    }
    public async Task Start(CancellationToken token)
    {
        await Task.Run(() =>
        {
            var settings = ApplicationSettings.GetInstance()!;
            var consumer = new SendEmailConsumer(settings.RabbitMq, _progress, _emailsService, _dbLogger, token);
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