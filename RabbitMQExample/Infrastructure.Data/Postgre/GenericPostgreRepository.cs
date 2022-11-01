using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgre;

public class GenericPostgreRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    protected GenericPostgreRepository(PostgreContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
 
    public async Task CreateAsync(TEntity item)
    {
        await _dbSet.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsItemExistsAsync(object keyObj)
    {
        if (keyObj is not int key)
        {
            return false;
        }
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == key) != null;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}