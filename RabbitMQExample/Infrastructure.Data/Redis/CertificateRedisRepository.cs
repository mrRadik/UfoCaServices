using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using StackExchange.Redis;

namespace Infrastructure.Data.Redis;

public class CertificateRedisRepository : GenericRedisRepository<CertificateEntity>, ICertificateRepository
{
    private readonly IDatabase _db;
    
    public CertificateRedisRepository(IConnectionMultiplexer redis) : base(redis)
    {
        _db = redis.GetDatabase();
    }
    public new async Task CreateAsync(CertificateEntity cert)
    {
        await _db.StringSetAsync(cert.Thumbprint.ToLower(), JsonSerializer.Serialize(cert));
    }
}