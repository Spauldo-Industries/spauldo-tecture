namespace spauldo_techture;
public class RepoFactory
{
    private readonly Dictionary<Type, Func<Task<object>>> _repoFactories = [];

    public void RegisterRepo<T>(Func<Task<IRepoEntityFramework<T>>> repoFactory)
        where T : class
    {
        _repoFactories[typeof(T)] = async () => await repoFactory();
    }

    public async Task<IRepoEntityFramework<T>> GetRepo<T>()
        where T : class
    {
        if (_repoFactories.TryGetValue(typeof(T), out var repoFactory))
        {
            return (IRepoEntityFramework<T>) await repoFactory();
        }

        throw new InvalidOperationException($"Repo for {typeof(T)} not registered.");
    }
}