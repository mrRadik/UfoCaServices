using Consumer.CertificateInstaller.Models;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;

namespace Consumer.CertificateInstaller;

public interface IInstallCertificateWorker : IBaseWorker
{
}

public class InstallCertificateWorker : IInstallCertificateWorker
{
    private readonly IDbLogger<InstallCertificateConsumer> _dbLogger;
    private readonly IProgress<string> _progress;
    private readonly BaseExchange<CertificateEvent> _exchange;
    private readonly CertificateInstallerSettings _settings;

    public InstallCertificateWorker(IDbLogger<InstallCertificateConsumer> dbLogger, 
        IProgress<string> progress, 
        BaseExchange<CertificateEvent> exchange,
        IOptions<CertificateInstallerSettings> settings)
    {
        _dbLogger = dbLogger;
        _progress = progress;
        _exchange = exchange;
        _settings = settings.Value;
    }
    public async Task Start(CancellationToken token)
    {
        await Task.Run(() =>
        {
            var consumer = new InstallCertificateConsumer(_progress, _dbLogger, _exchange, _settings, token);
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