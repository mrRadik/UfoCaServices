using System.Text;
using BusinessFacade.Models;
using Consumer.CertificateInstaller;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var settings = ApplicationSettings.GetInstance()!;
var token = new CancellationTokenSource().Token;
var progress = new Progress<string>(Console.WriteLine);
var consumer = new InstallCertificateConsumer(settings.RabbitMq, progress, token);
consumer.Start();
Console.ReadLine();