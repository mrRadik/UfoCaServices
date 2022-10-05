using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQBase.Interfaces;
using RabbitMQBase.Models;

namespace RabbitMQBase;

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
        channel.ExchangeDeclare(exchange: Constants.ExchangeName, type: ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: Constants.ExchangeName,
            routingKey: Constants.RoutingKey,
            basicProperties: null,
            body: body);
    }
}