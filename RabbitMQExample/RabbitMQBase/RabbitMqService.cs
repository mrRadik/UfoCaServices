using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQBase.Interfaces;
using RabbitMQBase.Models;

namespace RabbitMQBase;

public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitMqSettingsModel _settings;
    private static IConnection _connection = null!;
    
    public RabbitMqService(RabbitMqSettingsModel settings)
    {
        _settings = settings;
        _connection = RabbitMqConnection.GetConnection(settings.RabbitConnectionSettings);
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
        channel.ExchangeDeclare(exchange: _settings.RabbitExchangeSettings.ExchangeName, 
            type: _settings.RabbitExchangeSettings.ExchangeType);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: _settings.RabbitExchangeSettings.ExchangeName,
            routingKey: _settings.RoutingKey,
            basicProperties: null,
            body: body);
    }
}