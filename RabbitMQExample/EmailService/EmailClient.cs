using System.Net;
using System.Net.Mail;
using EmailService.Models;

namespace EmailService;

public class EmailClient
{
    private static EmailClient? _instance;
    private static readonly object LockObject = new object();
    public SmtpClient Client { get; }

    private EmailClient(SmtpSettings settings)
    {
        Client = CreateClient(settings);
    }

    public static EmailClient GetInstance(SmtpSettings settings)
    {
        if (_instance != null)
        {
            return _instance;
        }
        lock (LockObject)
        {
            _instance ??= new EmailClient(settings);
        }

        return _instance;
    }
    
    private static SmtpClient CreateClient(SmtpSettings settings)
    {
        return new SmtpClient
        {
            Credentials = new NetworkCredential(settings.UserName, settings.Password),
            Host = settings.Host,
            Port = settings.Port,
            EnableSsl = settings.Ssl,
            DeliveryMethod = (SmtpDeliveryMethod)settings.SmtpDeliveryMethod,
            PickupDirectoryLocation = settings.PickupDirectoryLocation
        };
    }
}