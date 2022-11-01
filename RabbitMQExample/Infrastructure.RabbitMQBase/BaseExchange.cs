using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQBase.Models;
using SystemFacade;

namespace RabbitMQBase;

public class BaseExchange<T> where T : BaseEvent
{
    public IModel Channel { get; private set; }
    public string Name { get; private set; }

    private readonly IConnection _connection;

    public BaseExchange(IOptions<RabbitMqSettings> settings)
    {
        _connection = RabbitMqConnection.GetConnection(settings.Value.RabbitConnectionSettings);
        Channel = _connection.CreateModel();
        Name = typeof(T).Name;
        
        Channel.ExchangeDeclare(exchange: Name, type: settings.Value.RabbitExchangeSettings.ExchangeType);
    }

    public void Dispose()
    {
        Channel.Close();
        Channel.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}