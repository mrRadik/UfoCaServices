namespace RabbitMQBase.Models;

public class ConsumerSettings
{
    public string RoutingKey { get; set; } = default!;
    public bool AutoAck { get; set; }
}