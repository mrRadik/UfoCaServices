namespace RabbitMQBase.Models;

public class RabbitMqSettings
{
    public RabbitConnectionSettings RabbitConnectionSettings { get; set; } = default!;
    public RabbitExchangeSettings RabbitExchangeSettings { get; set; } = default!;
}

public class RabbitConnectionSettings
{
    public string Host { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class RabbitExchangeSettings
{ 
    public string ExchangeType { get; set; } = default!;
}
