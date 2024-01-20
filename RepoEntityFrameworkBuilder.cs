using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace spauldo_techture;
public class RepoEntityFrameworkBuilder<TEntity>(
    DbContext dbContext,
    IQueryable<TEntity> query,
    Func<IQueryable<TEntity>, IQueryable<TEntity>> maxInclude,
    Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null,
    Expression<Func<TEntity, bool>> filter = null)
{
    private readonly IQueryable<TEntity> _query = query;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _maxInclude = maxInclude;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _include = include;
    private readonly Expression<Func<TEntity, bool>> _filter = filter;
    private readonly DbContext _dbContext = dbContext;

    public RepoEntityFrameworkBuilder<TEntity> With(Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
    {
        return new RepoEntityFrameworkBuilder<TEntity>(_dbContext, _query, _maxInclude, include, _filter);
    }

    public RepoEntityFrameworkBuilder<TEntity> WithAll()
    {
        return new RepoEntityFrameworkBuilder<TEntity>(_dbContext, _query, _maxInclude, _maxInclude, filter: _filter);
    }

    public RepoEntityFrameworkBuilder<TEntity> Where(Expression<Func<TEntity, bool>> filter)
    {
        return new RepoEntityFrameworkBuilder<TEntity>(_dbContext, _query, _maxInclude, _include, CombineFilters(_filter, filter));
    }

    public async Task<TEntity> ById(int id)
    {
        IQueryable<TEntity> query = _query;

        if (_filter != null)
            query = query.Where(_filter);

        if (_include != null)
            query = _include(query);

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name) == id);
    }

    public async Task<TEntity> FirstOrDefaultAsync()
    {
        IQueryable<TEntity> query = _query;

        if (_filter != null)
            query = query.Where(_filter);

        if (_include != null)
            query = _include(query);

        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<List<TEntity>> ToListAsync()
    {
        IQueryable<TEntity> query = _query;

        if (_filter != null)
            query = query.Where(_filter);

        if (_include != null)
            query = _include(query);

        return await query.ToListAsync().ConfigureAwait(false);
    }

    private static Expression<Func<TEntity, bool>> CombineFilters(
        Expression<Func<TEntity, bool>> existingFilter,
        Expression<Func<TEntity, bool>> newFilter)
    {
        if (existingFilter == null)
        {
            return newFilter;
        }
        else
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var combined = Expression.AndAlso(
                new ReplaceParameterVisitor(existingFilter.Parameters[0], parameter).Visit(existingFilter.Body),
                new ReplaceParameterVisitor(newFilter.Parameters[0], parameter).Visit(newFilter.Body)
            );
            return Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
        }
    }

    private class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter = oldParameter;
        private readonly ParameterExpression _newParameter = newParameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}