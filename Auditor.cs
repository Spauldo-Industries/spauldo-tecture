namespace spauldo_techture;
public interface IAuditor<TEntity>
{
    Task AuditInsert(TEntity entity, string note = null);
    Task AuditUpdate(TEntity entity, string note = null);
    Task AuditDelete(TEntity entity, string note = null);
}

public abstract class Auditor<TEntity, TModel, TDto, TAudit> : IAuditor<TEntity>
    where TEntity : class
    where TModel : class
    where TDto : class
    where TAudit : class
{
    private readonly MapperFactory _mapperFactory;
    private readonly RepoFactory _repoFactory;

    protected readonly Func<Task<IRepoEntityFramework<TAudit>>> GetRepo;
    protected readonly Func<Task<MapperYonHash<TEntity, TModel, TDto, TAudit>>> GetMapper;

    public Auditor(MapperFactory mapperFactory, RepoFactory repoFactory)
    {
        _mapperFactory = mapperFactory;
        _repoFactory = repoFactory;
        GetRepo = async () => await _repoFactory.GetRepo<TAudit>();
        GetMapper = async () => await _mapperFactory.GetMapper<TEntity, TModel, TDto, TAudit>();
    }

    private async Task AuditInternal(TEntity entity, char operation, string note = null)
    {
        var mapper = await GetMapper();
        var repo = await GetRepo();

        var auditRecord = await mapper.Map<TAudit>(entity);

        var noteProperty = typeof(TAudit).GetProperty("Note");
        var operationProperty = typeof(TAudit).GetProperty("Operation");
        var auditDateProperty = typeof(TAudit).GetProperty("AuditDate");

        noteProperty?.SetValue(auditRecord, note);
        operationProperty?.SetValue(auditRecord, operation);
        auditDateProperty?.SetValue(auditRecord, DateTime.Now);

        await repo.Insert(auditRecord);
    }

    public virtual async Task AuditInsert(TEntity entity, string note = null)
    {
        await AuditInternal(entity, 'I', note);
    }

    public virtual async Task AuditUpdate(TEntity entity, string note = null)
    {
        await AuditInternal(entity, 'U', note);
    }

    public virtual async Task AuditDelete(TEntity entity, string note = null)
    {
        await AuditInternal(entity, 'D', note);
    }
}