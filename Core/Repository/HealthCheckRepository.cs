using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core;

public class HealthCheckRepository : IHealthCheckRepository
{
    private readonly ILogger<HealthCheckRepository> _logger;
    private readonly IMongoDatabase _mongoDB;
    public HealthCheckRepository(ILogger<HealthCheckRepository> logger, IMongoDatabase mongoDB)
    {
        _logger = logger;
        _mongoDB = mongoDB;
    }

    public virtual async Task<bool> checkDBConnection()
    {
        try
        {
            await _mongoDB.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
