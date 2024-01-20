namespace spauldo_techture;
public interface ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit> 
: ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    Task<TDto> FirstOrDefaultAsync();
    Task<List<TDto>> ToListAsync();
}

public class LogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>(
    MapperFactory mapperFactory,
    RepoFactory repoFactory
) : LogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>>(mapperFactory, repoFactory), 
    ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>
        where TEntity : class
        where TModel  : class
        where TDto    : class
        where TAudit  : class
{
    public async Task<TDto> FirstOrDefaultAsync()
    {
        TEntity entity = WithAllRelationsFlg ? await FetchEntityWithAllRelations() : await FetchEntityWithSpecificRelations();
        return await MapToDto(entity);
    }

    public async Task<List<TDto>> ToListAsync()
    {
        List<TEntity> entities = WithAllRelationsFlg ? await FetchEntitiesWithAllRelations() : await FetchEntitiesWithSpecificRelations();
        return await MapToDto(entities);
    }

    private async Task<TEntity> FetchEntityWithAllRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        return await repo.Get().Where(Predicate).WithAll()
            .ById(Id ?? default) ?? await repo.Get().Where(Predicate).WithAll().FirstOrDefaultAsync();
    }

    private async Task<List<TEntity>> FetchEntitiesWithAllRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        return await repo.Get().Where(Predicate).WithAll().ToListAsync();
    }

    private async Task<TEntity> FetchEntityWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        return await repo.Get().Where(Predicate).With(Include)
            .ById(Id ?? default) ?? await repo.Get().Where(Predicate).With(Include).FirstOrDefaultAsync();
    }

    private async Task<List<TEntity>> FetchEntitiesWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        return await repo.Get().Where(Predicate).With(Include).ToListAsync();
    }
}