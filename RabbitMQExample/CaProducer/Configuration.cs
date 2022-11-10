using CaProducer.HttpClient;
using CaProducer.Models;
using Domain.Interfaces;
using Infrastructure.Business;
using Infrastructure.Data.Postgre;
using Infrastructure.Data.Redis;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;
using StackExchange.Redis;
using SystemFacade;

namespace CaProducer;

public static class Configuration
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
            })
            .ConfigureServices((builder, services) =>
            {
                ConfigureAppSettings(builder, services);
                ConfigureDatabase(builder, services);
                services.Configure<GosUslugiApi>(builder.Configuration.GetSection(nameof(GosUslugiApi)));
                services.AddHttpClient<ICertificateHttpClient, CertificateHttpClient>(client =>
                {
                    var baseUrl =  builder.Configuration.GetRequiredSection(nameof(GosUslugiApi)).Get<GosUslugiApi>()?.BaseUrl ?? "";
                    
                    client.BaseAddress = new Uri(baseUrl);
                });
                services.AddScoped<ICertificateService, CertificateService>();
                services.AddSingleton<DownloadCertificateHostedService>();
                services.AddHostedService<DownloadCertificateHostedService>();
                services.AddScoped(typeof(IRabbitMqService<>), typeof(RabbitMqService<>));
                services.AddScoped(typeof(BaseExchange<>));
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddScoped<IProgress<string>, ConsoleProgress>();
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            });
    }

    private static void ConfigureAppSettings(HostBuilderContext builder,IServiceCollection services)
    {
        services.Configure<CaProducerSettings>(builder.Configuration.GetSection(nameof(CaProducerSettings)));
        services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
    }
    
    private static void ConfigureDatabase(HostBuilderContext builder, IServiceCollection services)
    {
        var connectionStringName = builder.Configuration
            .GetRequiredSection(nameof(CaProducerSettings))
            .Get<CaProducerSettings>()?.ConnectionStringName;
        
        if (connectionStringName.IsNullOrWhiteSpace())
        {
            throw new Exception("Connection string not defined");
        }
        
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName!)!;
        
        switch (connectionStringName)
        {
            case "PostgreConnectionString":
                ConfigurePostgre(services, connectionString);
                break;
            case "RedisConnectionString":
                ConfigureRedis(services, connectionString);
                break;
            default:
                throw new Exception("Connection string not defined");
        }
    }
    
    private static void ConfigurePostgre(IServiceCollection services, string connectionStringName)
    {
        services.AddDbContext<PostgreContext>(c => c.UseNpgsql(connectionStringName));
        services.AddScoped<ILogsRepository, LogPostgreRepository>();
        services.AddScoped<ICertificateRepository, CertificatePostgreRepository>();
    }
    
    private static void ConfigureRedis(IServiceCollection services, string connectionStringName)
    {
        services.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect(connectionStringName));
        services.AddScoped<ILogsRepository, LogRedisRepository>();
        services.AddScoped<ICertificateRepository, CertificateRedisRepository>();
    }
}