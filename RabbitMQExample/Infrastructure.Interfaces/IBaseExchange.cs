using RabbitMQ.Client;

namespace Infrastructure.Interfaces;

public interface IBaseExchange: IDisposable
{
    IConnection Connection { get; }
    IModel Channel { get; }
    string Name { get; }
}