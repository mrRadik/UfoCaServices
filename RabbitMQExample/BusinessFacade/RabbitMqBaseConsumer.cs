using System.Text;
using BusinessFacade.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BusinessFacade;

public abstract class RabbitMqConsumerBase
{
    private readonly IConnection _connection;
    private readonly IProgress<string> _progress;
    private readonly CancellationToken _token;

    protected RabbitMqConsumerBase(RabbitMqModel settings, IProgress<string> progress, CancellationToken token)
    {
        _connection = RabbitMqConnection.GetInstance(settings).Connection;
        _progress = progress;
        _token = token;
    }
    public void Start()
    {
        using var channel = _connection.CreateModel();
        channel.ExchangeDeclare(exchange: Constants.ExchangeName, type: ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName,
            exchange: Constants.ExchangeName,
            routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += OnNewMessageReceived!;

        channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);

        _progress.Report($"Subscribed to the queue {queueName}");
        _progress.Report("Listening . . .");
        
        WaitHandle.WaitAny(new[] { _token.WaitHandle });
        
        consumer.Received -= OnNewMessageReceived!;
        channel.Close();
        channel.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    protected virtual void OnNewMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        _progress.Report($"Message: {Encoding.UTF8.GetString(e.Body.ToArray())}");
    }
}