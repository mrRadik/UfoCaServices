using BusinessFacade.Models;
using RabbitMQ.Client;

namespace BusinessFacade;

public class RabbitMqConnection
{
    private static RabbitMqConnection? _instance;
    private static readonly object LockObject = new object();
    private static RabbitMqModel _rabbitSettings = default!;
    
    public IConnection Connection { get; private set; }

    private RabbitMqConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitSettings.Host,
            UserName = _rabbitSettings.User,
            Password = _rabbitSettings.Password
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
            _rabbitSettings = settings;
            _instance ??= new RabbitMqConnection();
        }

        return _instance;
    }
}