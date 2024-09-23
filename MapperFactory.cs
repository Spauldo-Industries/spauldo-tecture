namespace spauldo_techture;
public class MapperFactory
{
    private readonly Dictionary<Type, Func<Task<object>>> _mapperFactories = [];

    public void RegisterMapper<TEntity, TModel, TDto, TAudit>(Func<Task<MapperYonHash<TEntity, TModel, TDto, TAudit>>> mapperFactory)
        where TEntity : class
        where TModel  : class
        where TDto    : class
        where TAudit  : class
    {
        _mapperFactories[typeof(MapperYonHash<TEntity, TModel, TDto, TAudit>)] = async () => await mapperFactory();
    }

    public async Task<MapperYonHash<TEntity, TModel, TDto, TAudit>> GetMapper<TEntity, TModel, TDto, TAudit>()
        where TEntity : class
        where TModel  : class
        where TDto    : class
        where TAudit  : class
    {
        if (_mapperFactories.TryGetValue(typeof(MapperYonHash<TEntity, TModel, TDto, TAudit>), out var repoFactory))
        {
            return (MapperYonHash<TEntity, TModel, TDto, TAudit>) await repoFactory();
        }

        throw new InvalidOperationException($"Mapper for {typeof(TEntity)} {typeof(TModel)} {typeof(TDto)} {typeof(TAudit)} not registered.");
    }
}