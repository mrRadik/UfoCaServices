namespace Infrastructure.Interfaces;

public interface IRabbitMqService<T>: IDisposable
{
    void SendMessage(object obj);
    void SendMessage(string message, string routingKey = "");
}