using BusinessFacade;
using BusinessFacade.Services;
using NotificationExchange;

namespace Consumer.CertificateInstaller;

public interface IInstallCertificateWorker : IBaseWorker
{
}

public class InstallCertificateWorker : IInstallCertificateWorker
{
    private readonly IDbLogger<InstallCertificateConsumer> _dbLogger;
    private readonly IProgress<string> _progress;

    public InstallCertificateWorker(IDbLogger<InstallCertificateConsumer> dbLogger, IProgress<string> progress)
    {
        _dbLogger = dbLogger;
        _progress = progress;
    }
    public async Task Start(CancellationToken token)
    {
        await Task.Run(() =>
        {
            var settings = ApplicationSettings.GetInstance()!;
            var exchange = new CertificateNotificationExchange();
            var consumer = new InstallCertificateConsumer(_progress, _dbLogger, exchange, token);
            try
            {
                consumer.SubscribeAndReceive(settings.InstallerSettings.RoutingKey, settings.InstallerSettings.AutoAck);
            }
            catch (Exception ex)
            {
                _progress.Report("Can't connect to Rabbit MQ queue. See logs");
                _dbLogger.LogError(ex.Message);
            }
        }, token);

    }
}