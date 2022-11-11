using System.Net.Mail;
using Consumer.EmailSender.Models;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SystemFacade;
using EmailService;
using EmailService.Interfaces;
using EmailService.Models;
using Infrastructure.Business;
using Infrastructure.Data.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQBase;
using RabbitMQBase.Models;
using Services.Interfaces;

namespace Consumer.EmailSender;

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
                var smtpSettings = builder.Configuration.GetSection(nameof(EmailSenderSettings))
                    .Get<EmailSenderSettings>().Smtp;
                
                ConfigureAppSettings(builder, services);
                CreateMessageFolder(smtpSettings);
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                ConfigureDatabase(services, connectionString);
                
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddScoped<ILogsRepository, LogPostgreRepository>();
                services.AddScoped<IProgress<string>, ConsoleProgress>();
                services.AddSingleton<ISmtpService, SmtpService>(_=> new SmtpService(smtpSettings));
                services.AddSingleton<EmailSenderHostedService>();
                services.AddHostedService<EmailSenderHostedService>();
                services.AddScoped(typeof(BaseExchange<>));
            });
    }
    
    private static void ConfigureAppSettings(HostBuilderContext builder,IServiceCollection services)
    {
        services.Configure<EmailSenderSettings>(builder.Configuration.GetSection(nameof(EmailSenderSettings)));
        services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
    }
    
    private static void ConfigureDatabase(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PostgreContext>(c => c.UseNpgsql(connectionString));
    }

    private static void CreateMessageFolder(SmtpSettings smtpSettings)
    {
        if (smtpSettings.SmtpDeliveryMethod == (int)SmtpDeliveryMethod.SpecifiedPickupDirectory)
        {
            Directory.CreateDirectory(smtpSettings.PickupDirectoryLocation);
        }
    }
}