using System.Text;
using BusinessFacade;
using BusinessFacade.Models;
using RabbitMQ.Client.Events;

namespace Consumer.CertificateInstaller;

public class InstallCertificateConsumer : RabbitMqConsumerBase
{
    private IProgress<string> _progress;
    public InstallCertificateConsumer(RabbitMqModel settings, IProgress<string> progress, CancellationToken token) : base(settings, progress, token)
    {
        _progress = progress;
    }

    protected override void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        base.OnNewMessageReceived(sender, e);
        //Install cert logic
    }
}