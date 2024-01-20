using System.Linq.Expressions;

namespace spauldo_techture;
public interface ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, TChild>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
    where TChild  : class, ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, TChild>
{
    TChild Where(Expression<Func<TEntity, bool>> predicate);
    TChild WithRelated(Func<IQueryable<TEntity>, IQueryable<TEntity>> include);
    TChild WithAllRelated();
    TChild ById(int id);
    TChild WithValidation();
}

public abstract class LogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, TChild>(
    MapperFactory mapperFactory,
    RepoFactory repoFactory
)
    : ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, TChild>
    where TEntity : class
    where TModel : class
    where TDto : class
    where TAudit : class
    where TChild : class, ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, TChild>
{
    protected IRepoBuilder<TEntity> RepoBuilder { get; private set; } = new RepoBuilder<TEntity>(repoFactory);
    protected readonly IMapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TDto> _mapperBuilder = new MapperYonHashBuilder<TEntity, TModel, TDto, TAudit, TDto>(mapperFactory);

    protected Expression<Func<TEntity, bool>> Predicate { get; private set; } = null;
    protected Func<IQueryable<TEntity>, IQueryable<TEntity>> Include { get; private set; } = null;
    protected bool WithAllRelationsFlg { get; private set; } = false;
    protected bool WithValidationFlg { get; private set; } = false;
    protected int? Id { get; private set; } = null;

    public virtual TChild Where(Expression<Func<TEntity, bool>> predicate)
    {
        Predicate = predicate;
        return this as TChild;
    }

    public virtual TChild WithRelated(Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
    {
        Include = include;
        return this as TChild;
    }

    public virtual TChild WithAllRelated()
    {
        WithAllRelationsFlg = true;
        return this as TChild;
    }

    public virtual TChild ById(int id)
    {
        Id = id;
        return this as TChild;
    }

    public virtual TChild WithValidation()
    {
        WithValidationFlg = true;
        return this as TChild;
    }

    protected async Task<TDto> MapToDto(TEntity entity)
    {
        var mapper = await _mapperBuilder.GetMapper();
        return WithValidationFlg
            ? await mapper.Build<TDto>(entity).WithValidation().Map()
            : await mapper.Map<TDto>(entity)
                ?? throw new NullReferenceException("Entity cannot be null in Queryable Crud Logic Wrapper");
    }

    protected async Task<List<TDto>> MapToDto(List<TEntity> entities)
    {
        var mapper = await _mapperBuilder.GetMapper();
        return WithValidationFlg
            ? await mapper.Build<TDto>(entities).WithValidation().MapList()
            : await mapper.MapList<TDto>(entities)
                ?? throw new NullReferenceException("Entity list cannot be null in Queryable Crud Logic Wrapper");
    }

    protected async Task<List<TDto>> MapToDto(List<TModel> models)
    {
        var mapper = await _mapperBuilder.GetMapper();
        return WithValidationFlg
            ? await mapper.Build<TDto>(models).WithValidation().MapList()
            : await mapper.MapList<TDto>(models)
                ?? throw new NullReferenceException("Model list cannot be null in Queryable Crud Logic Wrapper");
    }

    protected async Task<TDto> MapToDto(TModel model)
    {
        var mapper = await _mapperBuilder.GetMapper();
        return WithValidationFlg
            ? await mapper.Build<TDto>(model).WithValidation().Map()
            : await mapper.Map<TDto>(model)
                ?? throw new NullReferenceException("Model cannot be null in Queryable Crud Logic Wrapper");
    }
}