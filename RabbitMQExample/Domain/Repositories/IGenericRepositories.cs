﻿using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> CreateAsync(TEntity item);
    Task<TEntity> FindByIdAsync(int id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task RemoveAsync(TEntity item);
    Task UpdateAsync(TEntity item);
    Task<IReadOnlyList<TEntity>> GetWithIncludeAsync(params Expression<Func<TEntity, object>>[] includeProperties);
    Task<IReadOnlyList<TEntity>> GetWithIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

}