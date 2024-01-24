using System.Linq.Expressions;

namespace spauldo_techture;
public interface IRepoEntityFrameworkBuilder<TEntity, TChild>
    where TEntity : class
    where TChild  : class
{
    TChild Where(Expression<Func<TEntity, bool>> predicate);
    TChild With(Func<IQueryable<TEntity>, IQueryable<TEntity>> include);
    TChild WithAll();
    IQueryable<TEntity> GetQuery();
}

public abstract class RepoEntityFrameworkBuilder<TEntity, TChild>(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> maxRelations = null)
: IRepoEntityFrameworkBuilder<TEntity, TChild>
    where TEntity : class
    where TChild  : class
{
    private readonly IQueryable<TEntity> _query = query;
    private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _maxRelations = maxRelations;
    private Expression<Func<TEntity, bool>> _predicate = null;
    private Func<IQueryable<TEntity>, IQueryable<TEntity>> _include = null;
    protected bool WithAllRelationsFlg { get; private set; } = false;

    public virtual TChild Where(Expression<Func<TEntity, bool>> predicate)
    {
        _predicate = CombinePredicates(_predicate, predicate);
        return this as TChild;
    }

    public virtual TChild With(Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
    {
        _include = include;
        return this as TChild;
    }

    public virtual TChild WithAll()
    {
        _include = _maxRelations;
        return this as TChild;
    }

    public virtual IQueryable<TEntity> GetQuery()
    {
        IQueryable<TEntity> query = _query;

        if (_predicate != null)
            query = query.Where(_predicate);

        if (_include != null)
            query = _include(query);

        return query;
    }

    private static Expression<Func<TEntity, bool>> CombinePredicates(
        Expression<Func<TEntity, bool>> existingPredicate,
        Expression<Func<TEntity, bool>> newPredicate)
    {
        if (existingPredicate == null)
        {
            return newPredicate;
        }
        else
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var combined = Expression.AndAlso(
                new ReplaceParameterVisitor(existingPredicate.Parameters[0], parameter).Visit(existingPredicate.Body),
                new ReplaceParameterVisitor(newPredicate.Parameters[0], parameter).Visit(newPredicate.Body)
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