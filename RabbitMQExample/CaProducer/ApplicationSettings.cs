using CaProducer.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQBase.Models;

namespace CaProducer;

public class ApplicationSettings
{
    private static ApplicationSettings? _instance;
    private static readonly object LockObject = new object();
    public RabbitMqSettingsModel RabbitMq { get; }
    public GosUslugiApi GosUslugiApi { get; }
    public string ConnectionString { get; set; }
    public CaProducerSettings CaProducerSettings { get; set; }

    private ApplicationSettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        RabbitMq = configuration.GetRequiredSection("RabbitMQ").Get<RabbitMqSettingsModel>()!;
        GosUslugiApi = configuration.GetRequiredSection("GosUslugiApi").Get<GosUslugiApi>()!;
        ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
        CaProducerSettings = configuration.GetRequiredSection("CaProducer").Get<CaProducerSettings>()!;
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