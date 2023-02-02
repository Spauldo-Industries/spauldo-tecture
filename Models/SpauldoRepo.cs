using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;

namespace spauldo_tecture.Models
{
    public abstract class SpauldoRepo : IRepo
    {
        private readonly IConfiguration _config;
        public abstract string DbKey { get; }
        public MySqlConnection Connection => new MySqlConnection(_config.GetConnectionString(DbKey));

        public SpauldoRepo(IConfiguration config)
        {
            _config = config;
        }
        
        virtual public async Task<int?> Insert(IEntity entity) 
        { 
            using (var connection = Connection)
            {
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
                connection.Open();
                return await connection.InsertAsync<IEntity>(entity);
            }
        }
        virtual public async Task<int> Update(IEntity entity) 
        { 
            using (var connection = Connection)
            {
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
                connection.Open();
                return await connection.UpdateAsync<IEntity>(entity);
            }
        }
        virtual public async Task<IEntity> Select<TConcreteEntity>(int entityId)
            where TConcreteEntity : class, IEntity
        {
            using (var connection = Connection)
            {
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
                connection.Open();
                return await connection.GetAsync<TConcreteEntity>(entityId);
            }
        }
        virtual public async Task<int> Delete<TConcreteEntity>(int entityId)
            where TConcreteEntity : class, IEntity
        {
            using (var connection = Connection)
            {
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
                connection.Open();
                return await connection.DeleteAsync<TConcreteEntity>(entityId);
            }
        }
    }
}