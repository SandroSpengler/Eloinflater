using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Extension
{
    public static class InfrastructureServiceCollection
    {
        public static IServiceCollection SetupServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration["MongoDB:ConnectionString"];

            if (connectionString == null)
            {
                throw new ArgumentException(
                    "Missing required environment variable: MongoDB in Infrastructure" +
                    "\n " +
                    "Please check the documentation for a template or take a look at the default appsettings.json"
                );
            }

            MongoClient dbClient = new(configuration["MongoDB:ConnectionString"]);

            IMongoDatabase mongoDB = dbClient.GetDatabase("league");
            services.AddSingleton<IMongoDatabase>(mongoDB);

            return services;
        }
    }
}