using Domain.Interfaces;
using Infrastructure.Business;
using Infrastructure.Data;
using Infrastructure.Data.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using SystemFacade;

namespace Consumer.CertificateInstaller;

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
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddScoped<IInstallCertificateWorker, InstallCertificateWorker>();
                services.AddScoped<ILogsRepository, LogPostgreRepository>();
                services.AddScoped<IProgress<string>, ConsoleProgress>();
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<PostgreContext>(c => c.UseNpgsql(Settings.ConnectionString));
    }
}