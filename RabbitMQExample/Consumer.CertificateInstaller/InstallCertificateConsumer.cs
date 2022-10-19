using System.Text;
using Consumer.CertificateInstaller.Models;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQBase;
using Services.Interfaces;

namespace Consumer.CertificateInstaller;

public class InstallCertificateConsumer : RabbitMqConsumerBase
{
    private readonly IProgress<string> _progress;
    private readonly IDbLogger<InstallCertificateConsumer> _dbLogger;
    public InstallCertificateConsumer(
        IProgress<string> progress,
        IDbLogger<InstallCertificateConsumer> dbLogger,
        IBaseExchange exchange,
        CancellationToken token) : base(progress, exchange, token)
    {
        _progress = progress;
        _dbLogger = dbLogger;
    }

    protected override async void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        base.OnNewMessageReceived(sender, e);
        var settings = ApplicationSettings.GetInstance();
        var message = Encoding.Default.GetString(e.Body.ToArray());
        var certificate = JsonConvert.DeserializeObject<CertificateModel>(message);
        try
        {
            X509Helper.InstallCertificate(certificate.Data, settings.InstallerSettings.EmulateInstalling);
            _progress.Report($"The CA certificate {certificate.Subject} was installed successfully");
            
            if (!settings.InstallerSettings.AutoAck)
            {
                Channel.BasicAck(e.DeliveryTag, false);
            }
        }
        catch (Exception ex)
        {
            _progress.Report("Something went wrong. See logs");
            await _dbLogger.LogError(ex.Message);
        }
    }
}