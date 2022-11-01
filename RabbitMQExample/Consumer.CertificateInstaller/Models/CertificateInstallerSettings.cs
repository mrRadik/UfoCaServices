using RabbitMQBase.Models;

namespace Consumer.CertificateInstaller.Models;

public class CertificateInstallerSettings : ConsumerSettings
{
    public bool EmulateInstalling { get; set; }
}