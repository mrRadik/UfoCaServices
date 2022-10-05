using Consumer.EmailSender;
using Microsoft.Extensions.DependencyInjection;

var exitEvent = new ManualResetEvent(false);

var host = Configuration.CreateHostBuilder(args).Build();
var worker = host.Services.GetService<IEmailSenderWorker>()!;
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