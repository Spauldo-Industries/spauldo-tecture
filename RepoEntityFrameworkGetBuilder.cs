using Microsoft.EntityFrameworkCore;

namespace spauldo_techture;

public interface IRepoEntityFrameworkGetBuilder<TEntity>
: IRepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkGetBuilder<TEntity>>
    where TEntity : class
{
    Task<TEntity> FirstOrDefaultAsync();
    Task<List<TEntity>> ToListAsync();
    Task<TEntity> ById(int id);
}

public class RepoEntityFrameworkGetBuilder<TEntity>(DbContext dbContext, IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> maxRelations = null)
: RepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkGetBuilder<TEntity>>(query, maxRelations)
, IRepoEntityFrameworkGetBuilder<TEntity>
    where TEntity : class
{
    private readonly DbContext _dbContext = dbContext;

    public async Task<TEntity> ById(int id)
    {
        IQueryable<TEntity> query = GetQuery();
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name) == id);
    }

    public async Task<TEntity> FirstOrDefaultAsync()
    {
        IQueryable<TEntity> query = GetQuery();
        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<List<TEntity>> ToListAsync()
    {
        IQueryable<TEntity> query = GetQuery();
        return await query.ToListAsync().ConfigureAwait(false);
    }
}