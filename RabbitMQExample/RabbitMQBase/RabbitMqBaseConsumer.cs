using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQBase.Models;

namespace RabbitMQBase;

public abstract class RabbitMqConsumerBase
{
    private readonly IProgress<string> _progress;
    private readonly CancellationToken _token;
    private readonly RabbitMqSettingsModel _settings;
    public readonly IModel Channel;
    private readonly IConnection _connection;

    protected RabbitMqConsumerBase(RabbitMqSettingsModel settings, IProgress<string> progress, CancellationToken token)
    {
        _connection = RabbitMqConnection.GetConnection(settings.RabbitConnectionSettings);
        _settings = settings;
        _progress = progress;
        _token = token;
        Channel = _connection.CreateModel();
    }
    public void SubscribeAndReceive()
    {
        Channel.ExchangeDeclare(exchange: _settings.RabbitExchangeSettings.ExchangeName, 
            type: _settings.RabbitExchangeSettings.ExchangeType);

        var queueName = Channel.QueueDeclare().QueueName;

        Channel.QueueBind(queue: queueName,
            exchange: _settings.RabbitExchangeSettings.ExchangeName,
            routingKey: _settings.RoutingKey);
        
        var consumer = new EventingBasicConsumer(Channel);

        consumer.Received += OnNewMessageReceived!;

        Channel.BasicConsume(queue: queueName,
            autoAck: _settings.AutoAck, 
            consumer: consumer);
        
        _progress.Report($"Subscribed to the queue {queueName}");
        _progress.Report("Listening . . .");
        
        WaitHandle.WaitAny(new[] { _token.WaitHandle });
        
        _progress.Report("Stopping . . .");
        
        consumer.Received -= OnNewMessageReceived!;
        Channel.Close();
        Channel.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    protected virtual void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        _progress.Report($"New message: {Encoding.UTF8.GetString(e.Body.ToArray())}");
    }
}