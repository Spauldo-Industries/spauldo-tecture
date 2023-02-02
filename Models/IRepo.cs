using MySql.Data.MySqlClient;

namespace spauldo_tecture.Models
{
    public interface IRepo
    {
        string DbKey { get; }
        MySqlConnection Connection { get; }
        Task<int?> Insert(IEntity entity);
        Task<int> Update(IEntity entity);
        Task<IEntity> Select<TConcreteEntity>(int entityId) 
            where TConcreteEntity : class, IEntity;
        Task<int> Delete<TConcreteEntity>(int entityId) 
            where TConcreteEntity : class, IEntity;
    }
}