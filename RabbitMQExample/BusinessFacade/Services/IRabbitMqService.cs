namespace BusinessFacade.Services;

public interface IRabbitMqService
{
    void SendMessage(object obj);
    void SendMessage(string message);
}