namespace spauldo_techture;
public interface ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit> 
: ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    Task<TDto> FirstOrDefaultAsync();
    Task<Page<TDto>> ToListAsync();
    LogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit> WithPageSetting(int pageNumber = 1, int pageSize = 1000);
}

public class LogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>(MapperFactory mapperFactory, RepoFactory repoFactory) 
: LogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>>(mapperFactory, repoFactory)
, ILogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    private Page<TDto> Page { get; set; } = new Page<TDto>();

    public async Task<TDto> FirstOrDefaultAsync()
    {
        TEntity entity = WithAllRelationsFlg ? await FetchEntityWithAllRelations() : await FetchEntityWithSpecificRelations();
        return await MapToDto(entity);
    }

    public async Task<Page<TDto>> ToListAsync()
    {
        Page<TEntity> page = WithAllRelationsFlg ? await FetchEntitiesWithAllRelations() : await FetchEntitiesWithSpecificRelations();
        Page.Values = await MapToDto(page.Values);
        return Page;
    }

    public LogicCrudYonGetBuilder<TEntity, TModel, TDto, TAudit> WithPageSetting(int pageNumber = 1, int pageSize = 1000)
    {
        Page.PageSize = pageSize;
        Page.PageNumber = pageNumber;
        return this;
    }

    private async Task<TEntity> FetchEntityWithAllRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        if (Id.HasValue)
            return await repo.GetBuilder().Where(Predicate).WithAll().ById(Id.GetValueOrDefault());
        return await repo.GetBuilder().Where(Predicate).WithAll().FirstOrDefaultAsync();
    }

    private async Task<Page<TEntity>> FetchEntitiesWithAllRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        return await repo.GetBuilder()
            .WithPageSetting(Page.PageNumber, Page.PageSize)
            .Where(Predicate).WithAll()
            .ToListAsync();
    }

    private async Task<TEntity> FetchEntityWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        if (Id.HasValue)
            return await repo.GetBuilder().Where(Predicate).With(Include).ById(Id.GetValueOrDefault());
        return await repo.GetBuilder().Where(Predicate).With(Include).FirstOrDefaultAsync();
    }

    private async Task<Page<TEntity>> FetchEntitiesWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        return await repo.GetBuilder()
            .WithPageSetting(Page.PageNumber, Page.PageSize)
            .Where(Predicate).With(Include)
            .ToListAsync();
    }
}