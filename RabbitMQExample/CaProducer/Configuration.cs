﻿using CaProducer.HttpClient;
using Domain.Interfaces;
using Infrastructure.Business;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationExchange;
using RabbitMQBase;
using Services.Interfaces;
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
                services.AddScoped<ILogsRepository, LogsRepository>();
                services.AddScoped<ICertificateRepository, CertificatesRepository>();
                services.AddScoped<ICertificateService, CertificateService>();
                services.AddScoped<IDownloadCertificateWorker, DownloadCertificateWorker>();
                services.AddScoped<IRabbitMqService, RabbitMqService>(_ => new RabbitMqService(new CertificateNotificationExchange()));
                services.AddScoped(typeof(IDbLogger<>), typeof(DbLogger<>));
                services.AddScoped<IProgress<string>, ConsoleProgress>();
            });
    }
    
    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<DomainContext>(c => c.UseNpgsql(Settings.ConnectionString));
    }
}