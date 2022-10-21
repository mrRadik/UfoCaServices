using Domain.Interfaces;
using Domain.Models;
using StackExchange.Redis;

namespace Infrastructure.Data.Redis;

public class LogRedisRepository : GenericRedisRepository<LogEntity>, ILogsRepository
{
    public LogRedisRepository(IConnectionMultiplexer redis) : base(redis)
    {
    }
}