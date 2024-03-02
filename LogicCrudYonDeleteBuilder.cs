namespace spauldo_techture;

public interface ILogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>
: ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    Task DeleteAsync();
}

public class LogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>
: LogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>>
, ILogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    private readonly AuditorFactory _auditorFactory;
    private readonly Func<Task<IAuditor<TEntity>>> GetAuditor;

    public LogicCrudYonDeleteBuilder(MapperFactory mapperFactory, RepoFactory repoFactory, AuditorFactory auditorFactory)
    : base (mapperFactory, repoFactory)
    {
        _auditorFactory = auditorFactory;
        GetAuditor = async () => await _auditorFactory.GetAuditor <TEntity, TModel, TDto, TAudit>();
    }
    
    public async Task DeleteAsync()
    {
        if (WithAllRelationsFlg)
        {
            await DeleteWithAllRelations();
            return;
        }
            
        await DeleteWithSpecificRelations();
    }

    private async Task DeleteWithAllRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        var auditor = await GetAuditor();

        var entity = await repo.GetBuilder().ById(Id ?? default);
        entity ??= await repo.GetBuilder().Where(Predicate).With(Include).FirstOrDefaultAsync();

        if (Id != null) {
            await repo.DeleteBuilder().Where(Predicate).WithAll().ById(Id ?? default);
            await auditor.AuditDelete(entity, $"Deleted {entity.GetType().Name} where: {Predicate}, withAll, byId: {Id ?? default}");
            return;
        }
        await repo.DeleteBuilder().Where(Predicate).WithAll().DeleteAsync();
        await auditor.AuditDelete(entity, $"Deleted {entity.GetType().Name} where: {Predicate}, withAll");
    }

    private async Task DeleteWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        var auditor = await GetAuditor();

        var entity = await repo.GetBuilder().ById(Id ?? default);
        entity ??= await repo.GetBuilder().Where(Predicate).With(Include).FirstOrDefaultAsync();

        if (Id != null) {
            await repo.DeleteBuilder().Where(Predicate).With(Include).ById(Id ?? default);
            await auditor.AuditDelete(entity, $"Deleted {entity.GetType().Name} where: {Predicate}, with: {Include}, byId: {Id ?? default}");
            return;
        }
        await repo.DeleteBuilder().Where(Predicate).With(Include).DeleteAsync();
        await auditor.AuditDelete(entity, $"Deleted {entity.GetType().Name} where: {Predicate}, with: {Include}");
    }
}