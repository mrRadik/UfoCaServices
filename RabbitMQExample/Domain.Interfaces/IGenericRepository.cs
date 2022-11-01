using Domain.Models;

namespace Domain.Interfaces;

public interface IGenericRepository<TEntity> : IDisposable where TEntity : BaseEntity
{
    Task CreateAsync(TEntity item);
    Task<bool> IsItemExistsAsync(object key);

}