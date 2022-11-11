using Consumer.CertificateInstaller.Models;
using Domain.Interfaces;
using Infrastructure.Business;
using Infrastructure.Data.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;
using SystemFacade;

namespace Consumer.CertificateInstaller;

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
                
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                ConfigureDatabase(services, connectionString);
                
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddSingleton<InstallCertificateHostedService>();
                services.AddHostedService<InstallCertificateHostedService>();
                services.AddScoped<ILogsRepository, LogPostgreRepository>();
                services.AddScoped<IProgress<string>, ConsoleProgress>();
                services.AddScoped(typeof(BaseExchange<>));
            });
    }
    
    private static void ConfigureAppSettings(HostBuilderContext builder,IServiceCollection services)
    {
        services.Configure<CertificateInstallerSettings>(builder.Configuration.GetSection(nameof(CertificateInstallerSettings)));
        services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
    }
    private static void ConfigureDatabase(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PostgreContext>(c => c.UseNpgsql(connectionString));
    }
}