using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using CaProducer;
using CaProducer.HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

class Program
{

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var caHttpClient = host.Services.GetService<ICertificateHttpClient>();
        if (caHttpClient == null)
        {
            return;
        }
        
        //var b = await caHttpClient.DownloadCertificate(34749);
        var certList = await caHttpClient.GetCertList();
        foreach (var cert in certList.Data)
        {
            var a = await caHttpClient.DownloadCertificate(cert.Id);
            Console.WriteLine($"{cert.CertInfo.Subject} {a}");
        }
        Console.ReadKey();
    }

    static IHostBuilder CreateHostBuilder(string[] args)
    {
        var settings = new ApplicationSettings().GosUslugiApi;

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<ICertificateHttpClient, CertificateHttpClient>();
                services.AddHttpClient<ICertificateHttpClient, CertificateHttpClient>(client =>
                {
                    client.BaseAddress = new Uri(settings.BaseUrl);
                });
            });
    }

    static Func<Task> CreateTask(int timeToSleepTo, string routingKey)
    {
        return () =>
        {
            var counter = 0;
            do
            {
                int timeToSleep = new Random().Next(1000, timeToSleepTo);
                Thread.Sleep(timeToSleep);
                var rabbitSettings = new ApplicationSettings().RabbitMq;
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitSettings.Host,
                    UserName = rabbitSettings.User,
                    Password = rabbitSettings.Password
                };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: "notifier", type: ExchangeType.Fanout);

                string message = $"Message type [{routingKey}] from publisher N {counter}";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct_logs",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"Message type [{routingKey}] is sent into Direct Exchange [N:{counter++}]");
            } while (true);
        };
    }
}



