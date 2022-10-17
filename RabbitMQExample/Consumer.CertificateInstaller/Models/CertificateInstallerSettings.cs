namespace Consumer.CertificateInstaller.Models;

internal class CertificateInstallerSettings
{
    public bool EmulateInstalling { get; set; }
    public string RoutingKey { get; set; } = default!;
    public bool AutoAck { get; set; }
}