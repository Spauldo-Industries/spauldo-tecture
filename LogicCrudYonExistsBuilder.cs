namespace spauldo_techture;

public interface ILogicCrudYonExistsBuilder<TEntity, TModel, TDto, TAudit>
: ILogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonExistsBuilder<TEntity, TModel, TDto, TAudit>>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    Task<bool> ExistsAsync();
}

public class LogicCrudYonExistsBuilder<TEntity, TModel, TDto, TAudit>(MapperFactory mapperFactory, RepoFactory repoFactory) 
: LogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonExistsBuilder<TEntity, TModel, TDto, TAudit>>(mapperFactory, repoFactory)
, ILogicCrudYonExistsBuilder<TEntity, TModel, TDto, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
    public async Task<bool> ExistsAsync()
    {
        if (WithAllRelationsFlg)
            return await ExistsWithAllRelations();
        return await ExistsWithSpecificRelations();
    }

    private async Task<bool> ExistsWithAllRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        if (Id != null) 
            return await repo.ExistsBuilder().Where(Predicate).WithAll().ById(Id ?? default);
        return await repo.ExistsBuilder().Where(Predicate).WithAll().ExistsAsync();
    }

    private async Task<bool> ExistsWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        if (Id != null) 
            return await repo.ExistsBuilder().Where(Predicate).With(Include).ById(Id ?? default);
        return await repo.ExistsBuilder().Where(Predicate).With(Include).ExistsAsync();
    }
}