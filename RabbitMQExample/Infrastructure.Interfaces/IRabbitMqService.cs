namespace Infrastructure.Interfaces;

public interface IRabbitMqService: IDisposable
{
    void SendMessage(object obj);
    void SendMessage(string message, string routingKey = "");
}