namespace spauldo_techture;
public class AuditorFactory
{
    private readonly Dictionary<Type, Func<Task<object>>> _auditorFactories = [];

    public void RegisterAuditor<TEntity, TModel, TDto, TAudit>(Func<Task<Auditor<TEntity, TModel, TDto, TAudit>>> auditorFactory)
        where TEntity : class
        where TModel  : class
        where TDto    : class
        where TAudit  : class
    {
        _auditorFactories[typeof(Auditor<TEntity, TModel, TDto, TAudit>)] = async () => await auditorFactory();
    }

    public async Task<Auditor<TEntity, TModel, TDto, TAudit>> GetAuditor<TEntity, TModel, TDto, TAudit>()
        where TEntity : class
        where TModel  : class
        where TDto    : class
        where TAudit  : class
    {
        if (_auditorFactories.TryGetValue(typeof(Auditor<TEntity, TModel, TDto, TAudit>), out var repoFactory))
        {
            return (Auditor<TEntity, TModel, TDto, TAudit>) await repoFactory();
        }

        throw new InvalidOperationException($"Auditor for {typeof(TEntity)} {typeof(TModel)} {typeof(TDto)} {typeof(TAudit)} not registered.");
    }
}