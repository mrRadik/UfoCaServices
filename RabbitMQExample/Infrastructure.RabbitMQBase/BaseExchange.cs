using Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQBase.Models;
using SystemFacade;

namespace RabbitMQBase;

public class BaseExchange<T> : IBaseExchange where T: class
{
    public IConnection Connection { get; private set; }
    public IModel Channel { get; private set; }
    public string Name { get; private set; }

    protected BaseExchange(RabbitMqSettingsModel settings)
    {
        Connection = RabbitMqConnection.GetConnection(settings.RabbitConnectionSettings);
        Channel = Connection.CreateModel();
        Name = settings.RabbitExchangeSettings.ExchangeName.IsNullOrWhiteSpace() 
            ? typeof(T).Name 
            : settings.RabbitExchangeSettings.ExchangeName;
        
        Channel.ExchangeDeclare(exchange: Name, type: settings.RabbitExchangeSettings.ExchangeType);
    }

    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();
        Channel.Close();
        Channel.Dispose();
    }
}