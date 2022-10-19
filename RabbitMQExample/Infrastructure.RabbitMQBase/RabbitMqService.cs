using System.Text;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMQBase;

public class RabbitMqService : IRabbitMqService
{
    private readonly IBaseExchange _exchange;

    public RabbitMqService(IBaseExchange exchange)
    {
        _exchange = exchange;
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

    public void SendMessage(string message, string routingKey = "")
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        _exchange.Channel.BasicPublish(exchange: _exchange.Name,
            routingKey: routingKey,
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _exchange.Dispose();
    }
}