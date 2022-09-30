namespace BusinessFacade;

public interface IBaseWorker
{
    Task Start(CancellationToken token);
}