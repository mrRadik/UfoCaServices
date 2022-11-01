using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQBase.Models;

namespace RabbitMQBase;

public abstract class RabbitMqConsumerBase<T> where T : BaseEvent
{
    private readonly IProgress<string> _progress;
    private readonly BaseExchange<T> _exchange;
    private readonly ConsumerSettings _settings;
    private readonly CancellationToken _token;
    private readonly IModel _channel;

    protected RabbitMqConsumerBase(IProgress<string> progress,
        BaseExchange<T> exchange,
        ConsumerSettings settings,
        CancellationToken token)
    {
        _progress = progress;
        _exchange = exchange;
        _settings = settings;
        _token = token;
        _channel = exchange.Channel;
    }
    public void SubscribeAndReceive()
    {
        var queueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(queue: queueName,
            exchange: _exchange.Name,
            routingKey: _settings.RoutingKey);
        
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += OnNewMessageReceived!;

        _channel.BasicConsume(queue: queueName,
            autoAck: _settings.AutoAck, 
            consumer: consumer);
        
        _progress.Report($"Subscribed to the queue {queueName}");
        _progress.Report("Listening . . .");
        
        WaitHandle.WaitAny(new[] { _token.WaitHandle });
        
        _progress.Report("Stopping . . .");
        
        consumer.Received -= OnNewMessageReceived!;
        _exchange.Dispose();
    }

    private void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        var stringMessage = Encoding.Default.GetString(e.Body.ToArray());
        var message = JsonConvert.DeserializeObject<T>(stringMessage);
        
        try
        {
            HandleMessage(message, e);
            if (!_settings.AutoAck)
            {
                _channel.BasicAck(e.DeliveryTag, false);
            }
        }
        catch (Exception ex)
        {
            if (!_settings.AutoAck)
            {
                _channel.BasicNack(e.DeliveryTag, false, true);
            }
            HandleError(ex);
        }
    }

    protected virtual void HandleMessage(T message, BasicDeliverEventArgs e)
    {
        throw new NotImplementedException();
    }

    protected virtual void HandleError(Exception exception)
    {
        throw new NotImplementedException();
    }
}