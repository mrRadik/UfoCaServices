using CaProducer;
using Domain.Models;
using Infrastructure.Data.Redis;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

var exitEvent = new ManualResetEvent(false);

var host = Configuration.CreateHostBuilder(args).Build();

var worker = host.Services.GetService<IDownloadCertificateWorker>()!;
var cancellationTokenSource = new CancellationTokenSource();
var token = cancellationTokenSource.Token;

Console.CancelKeyPress += (sender, eventArgs) => {
    eventArgs.Cancel = true;
    exitEvent.Set();
    cancellationTokenSource.Cancel();
};

await worker.Start(token);

exitEvent.WaitOne();

cancellationTokenSource.Dispose();