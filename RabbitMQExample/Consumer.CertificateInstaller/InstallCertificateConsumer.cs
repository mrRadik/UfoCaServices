using Consumer.CertificateInstaller.Models;
using RabbitMQ.Client.Events;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;

namespace Consumer.CertificateInstaller;

public class InstallCertificateConsumer : RabbitMqConsumerBase<CertificateEvent>
{
    private readonly IProgress<string> _progress;
    private readonly IDbLogger<InstallCertificateConsumer> _dbLogger;
    private readonly CertificateInstallerSettings _settings;

    public InstallCertificateConsumer(
        IProgress<string> progress,
        IDbLogger<InstallCertificateConsumer> dbLogger,
        BaseExchange<CertificateEvent> exchange,
        CertificateInstallerSettings settings,
        CancellationToken token) : base(progress, exchange, settings, token)
    {
        _progress = progress;
        _dbLogger = dbLogger;
        _settings = settings;
    }

    protected override void HandleMessage(CertificateEvent cert, BasicDeliverEventArgs e)
    {
        X509Helper.InstallCertificate(cert.Data, _settings.EmulateInstalling);
        _progress.Report($"The CA certificate {cert.Subject} was installed successfully");
    }

    protected override async void HandleError(Exception exception)
    {
        _progress.Report("Something went wrong. See logs");
        await _dbLogger.LogError(exception.Message);
    }
}