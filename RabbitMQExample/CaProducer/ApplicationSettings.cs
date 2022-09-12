using Microsoft.Extensions.Configuration;

namespace CaProducer;

public class ApplicationSettings
{
    private IConfiguration _configuration;
    public RabbitMq RabbitMq { get; }
    public GosUslugiApi GosUslugiApi { get; }

    public ApplicationSettings()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        
        RabbitMq = _configuration.GetRequiredSection("RabbitMQ").Get<RabbitMq>()!;
        GosUslugiApi = _configuration.GetRequiredSection("GosUslugiApi").Get<GosUslugiApi>()!;
    }
}

public class RabbitMq
{
    public string Host { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}
public class GosUslugiApi
{
    public string BaseUrl { get; set; } = default!;
    public string DownloadCertUrl { get; set; } = default!;
    public string GetCertListUrl { get; set; } = default!;
}