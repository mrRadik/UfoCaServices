namespace Infrastructure.Interfaces;

public interface IBaseWorker
{
    Task Start(CancellationToken token);
}