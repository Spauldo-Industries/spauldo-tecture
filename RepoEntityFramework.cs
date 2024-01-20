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
    RepoEntityFrameworkBuilder<TEntity> Get();

    /// <summary>
    /// Deletes an entity from the repository based on its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    Task Delete(int id);

    /// <summary>
    /// Deletes one or more entities from the repository based on a filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    Task DeleteFiltered(Expression<Func<TEntity, bool>> filter);

    /// <summary>
    /// Checks if an entity with the specified ID exists in the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to check.</param>
    /// <returns>True if the entity exists; otherwise, false.</returns>
    Task<bool> Exists(int id);

    /// <summary>
    /// Checks if an entity with the exists in the repository based on a filter expression.
    /// </summary>
    /// <param name="filter">The filter expression to apply.</param>
    Task<bool> Exists(Expression<Func<TEntity, bool>> filter);
}

public abstract class RepoEntityFramework<TEntity>(
    DbContext dbContext,
    Func<IQueryable<TEntity>, IQueryable<TEntity>> allRelatedEntities = null
    ) : IRepoEntityFramework<TEntity>
    where TEntity : class
{
    private readonly DbContext _dbContext = dbContext;
    protected readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> AllRelatedEntities = allRelatedEntities;

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
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateRange(List<TEntity> entities)
    {
        if (entities == null || entities.Count == 0)
            return;
        
        foreach (var entity in entities)
            _dbContext.Entry(entity).State = EntityState.Modified;

        await _dbContext.SaveChangesAsync();
    }

    public virtual RepoEntityFrameworkBuilder<TEntity> Get()
    {       
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        return new RepoEntityFrameworkBuilder<TEntity>(_dbContext, query, maxInclude: AllRelatedEntities);
    }

    public virtual async Task Delete(int id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity != null)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteFiltered(Expression<Func<TEntity, bool>> filter)
    {
        var entities = await _dbContext.Set<TEntity>()
            .Where(filter)
            .ToListAsync()
            .ConfigureAwait(false);
        if (entities.Count > 0)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public virtual async Task<bool> Exists(int id)
    {
        return await _dbContext.Set<TEntity>().AnyAsync(e => EF.Property<int>(e, _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name) == id);
    }

    public async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
    {
        var entities = await _dbContext.Set<TEntity>()
            .Where(filter)
            .ToListAsync()
            .ConfigureAwait(false);
        return entities.Count > 0;
    }
}