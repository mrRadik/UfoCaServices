using System.Text;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQBase.Models;

namespace RabbitMQBase;

public class RabbitMqService<T> : IRabbitMqService<T> where T : BaseEvent
{
    private readonly BaseExchange<T> _exchange;

    public RabbitMqService(BaseExchange<T> exchange)
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