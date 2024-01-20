using System.Linq.Expressions;
using HashidsNet;

namespace spauldo_techture;
public interface ILogicCrudYon<TDto, TModel, TEntity, TAudit> : ILogicHashId
    where TDto    : class
    where TModel  : class
    where TEntity : class
    where TAudit  : class
{
    ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit> BuildGet();
    Task<string> GetAsync(TDto dto);
    Task<string> SaveAsync(TModel model);
    // Shorthand for querying for entity by id with no related entities
    Task<TDto> GetAsync(string id);
    // Shorthand for querying for entity by id with no related entities
    Task<TDto> GetAsync(int id);
    // Shorthand for querying all with no related entities
    Task<List<TDto>> GetAll();
    Task DeleteAsync(string id);
    Task DeleteAsync(int id);
    Task DeleteAsync(Expression<Func<TEntity, bool>> filter);
    Task<bool> ExistsAsync(string id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
}

public abstract class LogicCrudYon<TEntity, TModel, TDto, TAudit> : LogicHashId, ILogicCrudYon<TDto, TModel, TEntity, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    private readonly RepoFactory _repoFactory;
    private readonly MapperFactory _mapperFactory;
    private readonly AuditorFactory _auditorFactory;
    private readonly IHashids _hashids;
    
    protected readonly Func<Task<IAuditor<TEntity>>> GetAuditor;
    protected readonly Func<Task<IRepoEntityFramework<TEntity>>> GetRepo;
    protected readonly Func<Task<MapperYonHash<TEntity, TModel, TDto, TAudit>>> GetMapper;

    public LogicCrudYon
    (
        RepoFactory repoFactory,
        MapperFactory mapperFactory,
        AuditorFactory auditorFactory,
        IHashids hashids
    ) : base (hashids)
    {
        _repoFactory = repoFactory;
        _mapperFactory = mapperFactory;
        _auditorFactory = auditorFactory;
        _hashids = hashids;
        GetAuditor = async () => await _auditorFactory.GetAuditor <TEntity, TModel, TDto, TAudit>();
        GetRepo = async () => await _repoFactory.GetRepo<TEntity>();
        GetMapper = async () => await _mapperFactory.GetMapper<TEntity, TModel, TDto, TAudit>();
    }

    public virtual ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit> BuildGet()
    {
        return new LogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>(_mapperFactory, _repoFactory);
    }

    public virtual async Task<string> GetAsync(TDto dto)
    {
        return await SaveInternal(dto);
    }

    public virtual async Task<string> SaveAsync(TModel model)
    {
        return await SaveInternal(model);
    }

    private async Task<string> SaveInternal(object source)
    {
        var mapper = await GetMapper();
        var auditor = await GetAuditor();
        var mappedEntity = await mapper.Map<TEntity>(source);
        
        await InsertOrUpdate(mappedEntity);
        await auditor.AuditInsert(mappedEntity, $"Saved {source.GetType().Name}.");

        // Map the entity back to DTO
        var mappedDto = await mapper.Map<TDto>(mappedEntity);

        // Get the ID property value from the mapped DTO
        var idPropertyValue = ModelHelper.GetPropertyValue(mappedDto, "Id");
        return idPropertyValue.ToString();
    }

    private async Task InsertOrUpdate(TEntity mappedEntity)
    {
        var repo = await GetRepo();

        // Extract the ID property value from the mapped entity
        if (!ModelHelper.TryGetIntPropertyValue(mappedEntity, "Id", out int entityId))
            throw new Exception($"Mapped entity of type {typeof(TEntity).Name} must have a valid value for the ID property.");

        if (entityId == 0)
        {
            await repo.Insert(mappedEntity);
            return;
        }
            
        await repo.Update(mappedEntity);
    }

    public virtual async Task<TDto> GetAsync(string id)
    {
        return await GetAsync(DecodeId(id));
    }
    
    public virtual async Task<TDto> GetAsync(int id)
    {
        return await BuildGet().ById(id).FirstOrDefaultAsync();
    }

    public virtual async Task<List<TDto>> GetAll()
    {
        return await BuildGet().ToListAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        IRepoEntityFramework<TEntity> repo = await GetRepo();
        await repo.Delete(id);
    }
    
    public virtual async Task DeleteAsync(string id)
    {
        await DeleteAsync(_hashids.Decode(id).FirstOrDefault());
    }

    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> filter)
    {
        IRepoEntityFramework<TEntity> repo = await GetRepo();
        await repo.DeleteFiltered(filter);
    }
    
    public virtual async Task<bool> ExistsAsync(int id)
    {
        IRepoEntityFramework<TEntity> repo = await GetRepo();
        return await repo.Exists(id);
    }
    
    public virtual async Task<bool> ExistsAsync(string id)
    {
        return await ExistsAsync(_hashids.Decode(id).FirstOrDefault());
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
    {
        IRepoEntityFramework<TEntity> repo = await GetRepo();

        return await repo.Exists(filter);
    }
}