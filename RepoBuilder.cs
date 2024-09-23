namespace spauldo_techture;
public interface IRepoBuilder<TEntity>
    where TEntity : class
{
    Task<IRepoEntityFramework<TEntity>> GetRepo();
}

public class RepoBuilder<TEntity>(RepoFactory repoFactory)
: IRepoBuilder<TEntity>
    where TEntity : class
{
    private readonly RepoFactory _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));

    public async Task<IRepoEntityFramework<TEntity>> GetRepo()
    {
        return await _repoFactory.GetRepo<TEntity>();
    }
}