using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace spauldo_techture;
/// <summary>
/// Represents a generic repository interface for basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity in the repository.</typeparam>
public interface IRepoEntityFramework<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Retrieves a filtered range of entities from the repository based on a filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <returns>A filtered list of entities.</returns>
    RepoEntityFrameworkGetBuilder<TEntity> GetBuilder();

    /// <summary>
    /// Inserts a single entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    Task Insert(TEntity entity);

    /// <summary>
    /// Inserts a range of entities into the repository.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    Task InsertRange(List<TEntity> entities);

    /// <summary>
    /// Updates a single entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task Update(TEntity entity);

    /// <summary>
    /// Updates a range of entities in the repository.
    /// </summary>
    /// <param name="entities">The list of entities to update.</param>
    Task UpdateRange(List<TEntity> entities);

    /// <summary>
    /// Retrieves a filtered range of entities from the repository based on a filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    /// <returns>A filtered list of entities.</returns>
    RepoEntityFrameworkDeleteBuilder<TEntity> DeleteBuilder();
    RepoEntityFrameworkExistsBuilder<TEntity> ExistsBuilder();
}

public abstract class RepoEntityFramework<TEntity>(
    DbContext dbContext,
    Func<IQueryable<TEntity>, IQueryable<TEntity>> allRelatedEntities = null
    ) : IRepoEntityFramework<TEntity>
    where TEntity : class
{
    private readonly DbContext _dbContext = dbContext;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _allRelatedEntities = allRelatedEntities;

    public virtual RepoEntityFrameworkGetBuilder<TEntity> GetBuilder()
    {       
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        return new RepoEntityFrameworkGetBuilder<TEntity>(_dbContext, query, _allRelatedEntities);
    }

    public virtual async Task Insert(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task InsertRange(List<TEntity> entities)
    {
        if (entities == null || entities.Count == 0)
            return;

        _dbContext.Set<TEntity>().AddRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task Update(TEntity entity)
    {
        var entry = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(e => e.Entity == entity);

        if (entry != null && entry.State == EntityState.Detached)
        {
            // The entity is not being tracked
            _dbContext.Update(entity);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateRange(List<TEntity> entities)
    {
        if (entities == null || entities.Count == 0)
            return;
        
        foreach (var entity in entities)
        {
            var entry = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(e => e.Entity == entity);

            if (entry != null && entry.State == EntityState.Detached)
            {
                // The entity is not being tracked
                _dbContext.Update(entity);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public virtual RepoEntityFrameworkDeleteBuilder<TEntity> DeleteBuilder()
    {       
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        return new RepoEntityFrameworkDeleteBuilder<TEntity>(_dbContext, query, _allRelatedEntities);
    }

    public virtual RepoEntityFrameworkExistsBuilder<TEntity> ExistsBuilder()
    {
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        return new RepoEntityFrameworkExistsBuilder<TEntity>(_dbContext, query, _allRelatedEntities);
    }
}