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

public class LogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>(MapperFactory mapperFactory, RepoFactory repoFactory) 
: LogicCrudYonBuilder<TEntity, TModel, TDto, TAudit, ILogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>>(mapperFactory, repoFactory)
, ILogicCrudYonDeleteBuilder<TEntity, TModel, TDto, TAudit>
    where TEntity : class
    where TModel  : class
    where TDto    : class
    where TAudit  : class
{
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
        if (Id != null) {
            await repo.DeleteBuilder().Where(Predicate).WithAll().ById(Id ?? default);
            return;
        }
        await repo.DeleteBuilder().Where(Predicate).WithAll().DeleteAsync();
    }

    private async Task DeleteWithSpecificRelations()
    {
        var repo = await RepoBuilder.GetRepo();
        if (Id != null) {
            await repo.DeleteBuilder().Where(Predicate).With(Include).ById(Id ?? default);
            return;
        }
        await repo.DeleteBuilder().Where(Predicate).With(Include).DeleteAsync();
    }
}