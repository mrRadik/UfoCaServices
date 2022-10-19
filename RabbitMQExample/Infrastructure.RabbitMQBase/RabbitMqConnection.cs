using RabbitMQ.Client;
using RabbitMQBase.Models;

namespace RabbitMQBase;

public class RabbitMqConnection
{
    private static RabbitMqConnection? _instance;
    private static readonly object LockObject = new object();
    
    public IConnection Connection { get; }

    private RabbitMqConnection(RabbitConnectionSettings settings)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password
        };
        
        Connection = factory.CreateConnection();
    }

    public static IConnection GetConnection(RabbitConnectionSettings settings)
    {
        return GetInstance(settings).Connection;
    }

    private static RabbitMqConnection GetInstance(RabbitConnectionSettings settings)
    {
        if (_instance != null)
        {
            return _instance;
        }
        lock (LockObject)
        {
            _instance ??= new RabbitMqConnection(settings);
        }

        return _instance;
    }
}