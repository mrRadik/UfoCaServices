using Microsoft.Extensions.Hosting;

namespace SystemFacade;

public abstract class BaseHostedService : IHostedService, IDisposable
{
    private readonly IProgress<string> _progress;
    private Task _task;
    private CancellationToken _cancellationToken;

    protected BaseHostedService(IProgress<string> progress)
    {
        _progress = progress;
    }
    
    protected virtual Task DoWork(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _progress.Report("Service started");
        _cancellationToken = cancellationToken;
        _task = DoWork(_cancellationToken);
        
        return _task.IsCompleted 
            ? _task 
            : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_task == null)
        {
            return;
        }
        
        _progress.Report("Service stopped");
    }

    public virtual void Dispose()
    {
        _task.Dispose();
    }
}