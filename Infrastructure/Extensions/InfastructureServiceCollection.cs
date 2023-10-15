using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection SetupServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            MongoClient dbClient = new(configuration["MongoDB:ConnectionString"]);

            IMongoDatabase mongoDB = dbClient.GetDatabase("league");

            services.AddSingleton<IMongoDatabase>(mongoDB);

            return services;
        }
    }
}
