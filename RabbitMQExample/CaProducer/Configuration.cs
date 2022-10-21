using CaProducer.HttpClient;
using Domain.Interfaces;
using Infrastructure.Business;
using Infrastructure.Data;
using Infrastructure.Data.Postgre;
using Infrastructure.Data.Redis;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationExchange;
using RabbitMQBase;
using Services.Interfaces;
using StackExchange.Redis;
using SystemFacade;

namespace CaProducer;

public static class Configuration
{
    private static ApplicationSettings Settings => ApplicationSettings.GetInstance()!;
    
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
            })
            .ConfigureServices(services =>
            {
                ConfigureDatabase(services);
                services.AddSingleton<ICertificateHttpClient, CertificateHttpClient>();
                services.AddHttpClient<ICertificateHttpClient, CertificateHttpClient>(client =>
                {
                    client.BaseAddress = new Uri(Settings.GosUslugiApi.BaseUrl);
                });
                services.AddScoped<ICertificateService, CertificateService>();
                services.AddScoped<IDownloadCertificateWorker, DownloadCertificateWorker>();
                services.AddScoped<IRabbitMqService, RabbitMqService>(_ => new RabbitMqService(new CertificateNotificationExchange()));
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddScoped<IProgress<string>, ConsoleProgress>();
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        switch (Settings.CaProducerSettings.ConnectionStringName)
        {
            case "PostgreConnectionString":
                ConfigurePostgre(services);
                break;
            case "RedisConnectionString":
                ConfigureRedis(services);
                break;
            default:
                throw new Exception("Connection string not defined");
        }
    }
    
    private static void ConfigurePostgre(IServiceCollection services)
    {
        services.AddDbContext<PostgreContext>(c => c.UseNpgsql(Settings.ConnectionString));
        services.AddScoped<ILogsRepository, LogPostgreRepository>();
        services.AddScoped<ICertificateRepository, CertificatePostgreRepository>();
    }
    
    private static void ConfigureRedis(IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var settingsConnectionString = Settings.ConnectionString;
            return ConnectionMultiplexer.Connect(settingsConnectionString);
        });
        services.AddScoped<ILogsRepository, LogRedisRepository>();
        services.AddScoped<ICertificateRepository, CertificateRedisRepository>();
    }
}