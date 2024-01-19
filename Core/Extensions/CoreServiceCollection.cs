using Core.Interfaces;
using Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;

namespace Core.Extensions
{
    public static class CoreServiceCollection
    {
        public static IServiceCollection SetupServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISummonerByLeagueRepository, SummonerByLeagueRepository>();
            services.AddSingleton<ISummonerRepository, SummonerRepository>();
            services.AddSingleton<IHealthCheckRepository, HealthCheckRepository>();
            services.AddSingleton<IDateService, DateService>();

            string? protocol = configuration["RiotGames:Protocol"];
            string? region = configuration["RiotGames:Region"];
            string? baseUrl = configuration["RiotGames:BaseUrl"];

            if ((protocol ?? region ?? baseUrl) == null)
            {
                throw new ArgumentException(
                    "Missing required environment variable: RiotGames in Dataminer => Program.cs" +
                    "\n " +
                    "Please check the documentation for a template or take a look at the default appsettings.json"
                );
            }

            string apiUrl = $"{protocol}://{region}.{baseUrl}";

            // var x = configuration["RiotGames:RGApiKey"]!;

            services.AddHttpClient(apiUrl)
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(apiUrl);
                c.DefaultRequestHeaders
                    .Add("X-Riot-Token", configuration["RiotGames:RGApiKey"]!);
            })
            .UseWithRestEaseClient<IRiotGamesApi>();

            return services;
        }
    }
}