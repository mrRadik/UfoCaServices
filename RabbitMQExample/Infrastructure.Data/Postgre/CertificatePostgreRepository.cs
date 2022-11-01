using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgre;

public class CertificatePostgreRepository : GenericPostgreRepository<CertificateEntity>, ICertificateRepository
{
    private readonly DbSet<CertificateEntity> _dbSet;
    
    public CertificatePostgreRepository(PostgreContext context) : base(context)
    {
        _dbSet = context.Set<CertificateEntity>();
    }

    public new async Task<bool> IsItemExistsAsync(object key)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Thumbprint == key) != null;
    }
}