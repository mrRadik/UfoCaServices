namespace RabbitMQBase.Models;

public class RabbitMqSettingsModel
{
    public RabbitConnectionSettings RabbitConnectionSettings { get; set; } = default!;
    public RabbitExchangeSettings RabbitExchangeSettings { get; set; } = default!;
    public string RoutingKey { get; set; } = default!;
    public bool AutoAck { get; set; } = default!;
}

public class RabbitConnectionSettings
{
    public string Host { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class RabbitExchangeSettings
{
    public string ExchangeName { get; set; } = default!;
    public string ExchangeType { get; set; } = default!;
}
