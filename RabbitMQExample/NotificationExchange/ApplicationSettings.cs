using Microsoft.Extensions.Configuration;
using RabbitMQBase.Models;

namespace NotificationExchange;

public class ApplicationSettings
{
    private static ApplicationSettings? _instance;
    private static readonly object LockObject = new object();
    public RabbitMqSettingsModel RabbitMq { get; }
    
    public ApplicationSettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("exchangeSettings.json")
            .Build();
        
        RabbitMq = configuration.GetRequiredSection("RabbitMq").Get<RabbitMqSettingsModel>()!;
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