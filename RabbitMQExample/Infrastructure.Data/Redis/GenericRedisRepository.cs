using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using StackExchange.Redis;

namespace Infrastructure.Data.Redis;

public class GenericRedisRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    protected GenericRedisRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task CreateAsync(TEntity item)
    {
        var stringItem = JsonSerializer.Serialize(item);
        await _db.StringSetAsync(item.Guid.ToString("D"), stringItem);
    }

    public async Task<bool> IsItemExistsAsync(object keyObj)
    {
        if (keyObj is not string key)
        {
            return false;
        }
        
        return await _db.KeyExistsAsync(key);
    }

    public void Dispose()
    {
        _redis.Close();
        _redis.Dispose();
    }
}