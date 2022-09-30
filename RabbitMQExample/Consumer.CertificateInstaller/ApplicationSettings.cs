using BusinessFacade.Models;
using Consumer.CertificateInstaller.Models;
using Microsoft.Extensions.Configuration;

namespace Consumer.CertificateInstaller;

public class ApplicationSettings
{
    private static ApplicationSettings? _instance;
    private static readonly object LockObject = new object();
    public RabbitMqModel RabbitMq { get; }
    public string ConnectionString { get; set; }
    public CertificateInstallerSettings InstallerSettings { get; set; }

    private ApplicationSettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        RabbitMq = configuration.GetRequiredSection("RabbitMQ").Get<RabbitMqModel>()!;
        ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
        InstallerSettings = configuration.GetRequiredSection("CertificateInstaller").Get<CertificateInstallerSettings>()!;
    }
    
    public static ApplicationSettings GetInstance()
    {
        if (_instance != null)
        {
            return _instance;
        }
        lock (LockObject)
        {
            _instance ??= new ApplicationSettings();
        }

        return _instance;
    }
}