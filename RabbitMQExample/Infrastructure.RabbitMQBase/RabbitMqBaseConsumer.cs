using System.Text;
using Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQBase;

public abstract class RabbitMqConsumerBase
{
    private readonly IProgress<string> _progress;
    private readonly IBaseExchange _exchange;
    private readonly CancellationToken _token;
    protected readonly IModel Channel;

    protected RabbitMqConsumerBase(IProgress<string> progress,
        IBaseExchange exchange,
        CancellationToken token)
    {
        _progress = progress;
        _exchange = exchange;
        _token = token;
        Channel = exchange.Channel;
    }
    public void SubscribeAndReceive(string routingKey ="", bool autoAck = false)
    {
        var queueName = Channel.QueueDeclare().QueueName;

        Channel.QueueBind(queue: queueName,
            exchange: _exchange.Name,
            routingKey: routingKey);
        
        var consumer = new EventingBasicConsumer(Channel);

        consumer.Received += OnNewMessageReceived!;

        Channel.BasicConsume(queue: queueName,
            autoAck: autoAck, 
            consumer: consumer);
        
        _progress.Report($"Subscribed to the queue {queueName}");
        _progress.Report("Listening . . .");
        
        WaitHandle.WaitAny(new[] { _token.WaitHandle });
        
        _progress.Report("Stopping . . .");
        
        consumer.Received -= OnNewMessageReceived!;
        _exchange.Dispose();
    }

    protected virtual void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        _progress.Report($"New message: {Encoding.UTF8.GetString(e.Body.ToArray())}");
    }
}