using System.Text;
using BusinessFacade.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BusinessFacade.Services.Implementations;

public class RabbitMqService : IRabbitMqService
{
    private static IConnection _connection = null!;
    
    public RabbitMqService(RabbitMqModel settings)
    {
        _connection = RabbitMqConnection.GetInstance(settings).Connection;
    }
    public void SendMessage(object obj)
    {
        var message = JsonConvert.SerializeObject(obj);
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }
        
        SendMessage(message);
    }

    public void SendMessage(string message)
    {
        using var channel = _connection.CreateModel();
        channel.ExchangeDeclare(exchange: "notifier", type: ExchangeType.Fanout);

        //string message = $"Message type [{routingKey}] from publisher N {counter}";

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "notifier",
            routingKey: "1",
            basicProperties: null,
            body: body);

        //Console.WriteLine($"Message type [{routingKey}] is sent into Direct Exchange [N:{counter++}]");
    }
}