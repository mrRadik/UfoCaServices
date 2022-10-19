using RabbitMQBase;
using RabbitMQBase.Models;

namespace NotificationExchange;

public class CertificateNotificationExchange : BaseExchange<CertificateNotificationExchange>
{
    private static readonly RabbitMqSettingsModel Settings;
    
    public CertificateNotificationExchange() : base(Settings)
    {
    }

    static CertificateNotificationExchange()
    {
        Settings = ApplicationSettings.GetInstance().RabbitMq;
    }
}