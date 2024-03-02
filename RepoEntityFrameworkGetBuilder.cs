using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace spauldo_techture;

public interface IRepoEntityFrameworkGetBuilder<TEntity>
: IRepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkGetBuilder<TEntity>>
    where TEntity : class
{
    Task<TEntity> FirstOrDefaultAsync();
    Task<TEntity> LastOrDefaultAsync();
    Task<Page<TEntity>> ToListAsync();
    Task<TEntity> ById(int id);
}

public class RepoEntityFrameworkGetBuilder<TEntity>(DbContext dbContext, IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> maxRelations = null)
: RepoEntityFrameworkBuilder<TEntity, IRepoEntityFrameworkGetBuilder<TEntity>>(query, maxRelations)
, IRepoEntityFrameworkGetBuilder<TEntity>
    where TEntity : class
{
    private readonly DbContext _dbContext = dbContext;
    private Page<TEntity> Page { get; set; } = new Page<TEntity>();

    public async Task<TEntity> ById(int id)
    {
        IQueryable<TEntity> query = GetQuery();
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name) == id);
    }

    public async Task<TEntity> FirstOrDefaultAsync()
    {
        var primaryKeyPropertyName = _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties
        .Select(x => x.Name).FirstOrDefault();
        Expression<Func<TEntity, object>> orderByExpression = 
            entity => EF.Property<object>(entity, primaryKeyPropertyName);
        IQueryable<TEntity> query = GetQuery().OrderBy(orderByExpression);
        var entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);
        _dbContext.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<TEntity> LastOrDefaultAsync()
    {
        var primaryKeyPropertyName = _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties
        .Select(x => x.Name).FirstOrDefault();
        Expression<Func<TEntity, object>> orderByExpression = 
            entity => EF.Property<object>(entity, primaryKeyPropertyName);
        IQueryable<TEntity> query = GetQuery().OrderByDescending(orderByExpression);
        var entity = await query.LastOrDefaultAsync().ConfigureAwait(false);
        _dbContext.Entry(entity).State = EntityState.Detached;
        return entity;
    }

    public async Task<Page<TEntity>> ToListAsync()
    {
        IQueryable<TEntity> query = GetQuery();
        Page.Values = await query.Skip((Page.PageNumber - 1) * Page.PageSize)
            .Take(Page.PageSize)
            .ToListAsync().ConfigureAwait(false);
        return Page;
    }

    public RepoEntityFrameworkGetBuilder<TEntity> WithPageSetting(int pageNumber = 1, int pageSize = 1000)
    {
        Page.PageSize = pageSize;
        Page.PageNumber = pageNumber;
        return this;
    }
}