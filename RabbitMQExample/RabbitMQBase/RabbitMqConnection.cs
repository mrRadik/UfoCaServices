using RabbitMQ.Client;
using RabbitMQBase.Models;

namespace RabbitMQBase;

public class RabbitMqConnection
{
    private static RabbitMqConnection? _instance;
    private static readonly object LockObject = new object();
    
    public IConnection Connection { get; }

    private RabbitMqConnection(RabbitMqModel settings)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password
        };
        
        Connection = factory.CreateConnection();
    }

    public static RabbitMqConnection GetInstance(RabbitMqModel settings)
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