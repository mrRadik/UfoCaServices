using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consumer.CertificateInstaller;

public static class Configuration
{
    private static ApplicationSettings Settings => ApplicationSettings.GetInstance()!;
    
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                ConfigureDatabase(services);
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<DomainContext>(c => c.UseNpgsql(Settings.ConnectionString));
    }
}