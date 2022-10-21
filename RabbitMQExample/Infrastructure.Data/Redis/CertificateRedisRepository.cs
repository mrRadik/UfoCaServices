using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using StackExchange.Redis;

namespace Infrastructure.Data.Redis;

public class CertificateRedisRepository : GenericRedisRepository<CertificateEntity>, ICertificateRepository
{
    public CertificateRedisRepository(IConnectionMultiplexer redis) : base(redis)
    {
    }
}