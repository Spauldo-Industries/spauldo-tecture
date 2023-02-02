using Microsoft.Extensions.DependencyInjection;
using HashidsNet;

namespace spauldo_tecture
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureSpauldoTechtureDi<TConcreteContainer, TConcrete>(this IServiceCollection services)
            where TConcreteContainer : class
            where TConcrete : class
        {
            services.AddScoped(typeof(TConcreteContainer));
            services.AddScoped(typeof(TConcrete));
        }

        public static void ConfigureSequentialIdEncoding(this IServiceCollection services, string salt)
        {
            services.AddTransient<IHashids>(x => new Hashids(salt, 11));
        }
    }
}