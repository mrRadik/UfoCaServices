using Consumer.CertificateInstaller.Models;
using Microsoft.Extensions.Options;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;
using SystemFacade;

namespace Consumer.CertificateInstaller;

public class InstallCertificateHostedService : BaseHostedService
{
    private readonly IDbLogger<InstallCertificateConsumer> _dbLogger;
    private readonly IProgress<string> _progress;
    private readonly BaseExchange<CertificateEvent> _exchange;
    private readonly CertificateInstallerSettings _settings;

    public InstallCertificateHostedService(IDbLogger<InstallCertificateConsumer> dbLogger, 
        IProgress<string> progress, 
        BaseExchange<CertificateEvent> exchange,
        IOptions<CertificateInstallerSettings> settings) : base(progress)
    {
        _dbLogger = dbLogger;
        _progress = progress;
        _exchange = exchange;
        _settings = settings.Value;
    }
    protected override async Task DoWork(CancellationToken token)
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

    public override void Dispose()
    {
        base.Dispose();
        _exchange.Dispose();
    }
}