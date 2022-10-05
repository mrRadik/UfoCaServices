﻿using System.Net.Mail;
using BusinessFacade.Services.Implementations;
using Domain;
using Domain.Repositories;
using Domain.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SystemFacade;
using BusinessFacade.Services;
using EmailService;
using EmailService.Interfaces;

namespace Consumer.EmailSender;

public static class Configuration
{
    private static ApplicationSettings Settings => ApplicationSettings.GetInstance()!;
    
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        CreateMessageFolder();
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
            })
            .ConfigureServices(services =>
            {
                ConfigureDatabase(services);
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddScoped<ILogsRepository, LogsRepository>();
                services.AddScoped<IProgress<string>, ConsoleProgress>();
                services.AddScoped<ISmtpService, SmtpService>(_=>new SmtpService(Settings.EmailSenderSettings.Smtp));
                services.AddScoped<IEmailSenderWorker, EmailSenderWorker>();
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<DomainContext>(c => c.UseNpgsql(Settings.ConnectionString));
    }

    private static void CreateMessageFolder()
    {
        if (Settings.EmailSenderSettings.Smtp.SmtpDeliveryMethod == (int)SmtpDeliveryMethod.SpecifiedPickupDirectory)
        {
            Directory.CreateDirectory(Settings.EmailSenderSettings.Smtp.PickupDirectoryLocation);
        }
    }
}