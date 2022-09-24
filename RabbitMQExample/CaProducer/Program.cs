using CaProducer;
using Microsoft.Extensions.DependencyInjection;

var host = Configuration.CreateHostBuilder(args).Build();
var worker = host.Services.GetService<IDownloadCertificateWorker>();
worker!.Start();
Console.ReadLine();