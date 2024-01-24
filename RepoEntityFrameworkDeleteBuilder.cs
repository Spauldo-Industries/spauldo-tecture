using Microsoft.EntityFrameworkCore;

namespace spauldo_techture;

public interface IRepoEntityFrameworkDeleteBuilder<TEntity>
: IRepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkDeleteBuilder<TEntity>>
    where TEntity : class
{
    Task ById(int id);
    Task DeleteAsync();
}

public class RepoEntityFrameworkDeleteBuilder<TEntity>(DbContext dbContext, IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> maxRelations = null)
: RepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkDeleteBuilder<TEntity>>(query, maxRelations)
, IRepoEntityFrameworkDeleteBuilder<TEntity>
    where TEntity : class
{
    private readonly DbContext _dbContext = dbContext;

    public async Task ById(int id)
    {
        IQueryable<TEntity> query = GetQuery();

        var entityToDelete = await query
            .FirstOrDefaultAsync(e => EF.Property<int>(e, _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name) == id)
            .ConfigureAwait(false);

        if (entityToDelete != null)
        {
            _dbContext.Entry(entityToDelete).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task DeleteAsync()
    {
        IQueryable<TEntity> query = GetQuery();

        var entitiesToDelete = await query.ToListAsync().ConfigureAwait(false);

        foreach (var entity in entitiesToDelete)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}