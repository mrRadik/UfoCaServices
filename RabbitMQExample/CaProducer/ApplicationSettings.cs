using CaProducer.Models;
using Microsoft.Extensions.Configuration;

namespace CaProducer;

public class ApplicationSettings
{
    private static ApplicationSettings? _instance;
    private static readonly object LockObject = new object();
    public GosUslugiApi GosUslugiApi { get; }
    public string ConnectionString { get; set; }
    public CaProducerSettings CaProducerSettings { get; set; }

    private ApplicationSettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        GosUslugiApi = configuration.GetRequiredSection("GosUslugiApi").Get<GosUslugiApi>()!;
        CaProducerSettings = configuration.GetRequiredSection("CaProducer").Get<CaProducerSettings>()!;
        ConnectionString = configuration.GetConnectionString(CaProducerSettings.ConnectionStringName)!;
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