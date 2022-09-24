using BusinessFacade.Models;
using Microsoft.Extensions.Configuration;

namespace Consumer.CertificateInstaller;

public class ApplicationSettings
{
    private static ApplicationSettings? _instance;
    private static readonly object LockObject = new object();
    public RabbitMqModel RabbitMq { get; }
    public string ConnectionString { get; set; }

    private ApplicationSettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        RabbitMq = configuration.GetRequiredSection("RabbitMQ").Get<RabbitMqModel>()!;
        ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
    }
    
    public static ApplicationSettings? GetInstance()
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