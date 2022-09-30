using BusinessFacade.Services.Implementations;
using Domain;
using Domain.Repositories;
using Domain.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                services.AddScoped<ILogsRepository, LogsRepository>();
                services.AddScoped<IProgress<string>, ConsoleProgress>();
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<DomainContext>(c => c.UseNpgsql(Settings.ConnectionString));
    }
}