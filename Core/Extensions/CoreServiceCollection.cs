using Core.Interfaces;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class CoreServiceCollection
    {
        public static IServiceCollection SetupServiceCollection(this IServiceCollection services)
        {
            services.AddSingleton<ISummonerByLeagueRepository, SummonerByLeagueRepository>();
            services.AddSingleton<ISummonerRepository, SummonerRepository>();

            return services;
        }
    }
}