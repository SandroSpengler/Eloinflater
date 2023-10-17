using Core.Interfaces;
using Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class CoreServiceCollection
    {
        public static IServiceCollection SetupServiceCollection(this IServiceCollection services)
        {
            services.AddSingleton<ISummonerByLeagueRepository, SummonerByLeagueRepository>();

            return services;
        }
    }
}