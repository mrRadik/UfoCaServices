namespace SystemFacade.Helpers;

public class ConsoleHelper<T>
{
    public void Info(string message)
    {
        Console.WriteLine($"{DateTime.Now} - {typeof(T).ToString()} - Info: {message}");
    }
}