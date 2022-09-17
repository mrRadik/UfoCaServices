using BusinessFacade.Services;
using BusinessFacade.Services.Implementations;
using CaProducer;
using CaProducer.HttpClient;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

var host = Configuration.CreateHostBuilder(args).Build();
var worker = host.Services.GetService<IDownloadCertificateWorker>();
worker!.Start();
Console.ReadLine();