using Microsoft.EntityFrameworkCore;

namespace spauldo_techture;

public interface IRepoEntityFrameworkExistsBuilder<TEntity>
: IRepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkExistsBuilder<TEntity>>
    where TEntity : class
{
    Task<bool> ById(int id);
    Task<bool> ExistsAsync();
}

public class RepoEntityFrameworkExistsBuilder<TEntity>(DbContext dbContext, IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> maxRelations = null)
: RepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkExistsBuilder<TEntity>>(query, maxRelations)
, IRepoEntityFrameworkExistsBuilder<TEntity>
    where TEntity : class
{
    private readonly DbContext _dbContext = dbContext;

    public async Task<bool> ById(int id)
    {
        IQueryable<TEntity> query = GetQuery();
        return await query.AnyAsync(e => EF.Property<int>(e, _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name) == id);
    }

    public async Task<bool> ExistsAsync()
    {
        IQueryable<TEntity> query = GetQuery();
        var entities = await query.ToListAsync().ConfigureAwait(false);
        return entities.Count > 0;
    }
}