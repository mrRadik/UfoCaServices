using System.Linq.Expressions;
using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using StackExchange.Redis;

namespace Infrastructure.Data.Redis;

public class GenericRedisRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly string _hashName;

    protected GenericRedisRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
        _hashName = $"{typeof(TEntity).Name.ToLower()}hashtable";
    }

    public void Dispose()
    {
        _redis.Close();
        _redis.Dispose();
    }

    public async Task CreateAsync(TEntity item)
    {
        var stringItem = JsonSerializer.Serialize(item);
        await _db.HashSetAsync(_hashName, new[] { new HashEntry(item.Guid.ToString("D"), stringItem) });
    }

    public async Task<TEntity?> FindByGuidAsync(Guid guid)
    {
        var item = await _db.HashGetAsync(_hashName, guid.ToString("D"));
        return !item.IsNullOrEmpty 
            ? JsonSerializer.Deserialize<TEntity>(item)
            : null;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        var items = (await GetAllASync())?.ToList();
        return items == null || !items.Any()
            ? new List<TEntity>()
            : items;
    }
    
    public Task<TEntity?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var items = await GetAllASync();
        
        return items != null
            ? items.Where(predicate).ToList()
            : new List<TEntity>();
    }

    public Task RemoveAsync(TEntity item)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TEntity item)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TEntity>> GetWithIncludeAsync(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TEntity>> GetWithIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        throw new NotImplementedException();
    }

    private async Task<IQueryable<TEntity>?> GetAllASync()
    {
        var items = await _db.HashGetAllAsync(_hashName);
        return !items.Any() 
            ? null
            : Array.ConvertAll(items, item => JsonSerializer.Deserialize<TEntity>(item.Value)).AsQueryable();
    }
}