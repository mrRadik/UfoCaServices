using BusinessFacade.Services;
using BusinessFacade.Services.Implementations;
using CaProducer.HttpClient;
using Domain;
using Domain.Repositories;
using Domain.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CaProducer;

public static class Configuration
{
    private static ApplicationSettings? Settings => ApplicationSettings.GetInstance();
    
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                ConfigureDatabase(services);
                services.AddSingleton<ICertificateHttpClient, CertificateHttpClient>();
                services.AddHttpClient<ICertificateHttpClient, CertificateHttpClient>(client =>
                {
                    client.BaseAddress = new Uri(Settings.GosUslugiApi.BaseUrl);
                });
                services.AddScoped<ILogsRepository, LogsRepository>();
                services.AddScoped<ICertificateRepository, CertificatesRepository>();
                services.AddScoped<ICertificateService, CertificateService>();
                services.AddScoped<IDownloadCertificateWorker, DownloadCertificateWorker>();
                services.AddScoped<IRabbitMqService, RabbitMqService>(_ => new RabbitMqService(Settings.RabbitMq));
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<DomainContext>(c => c.UseNpgsql(Settings.ConnectionString));
    }
}