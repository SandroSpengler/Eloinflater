using Core.Interfaces;
using Core.Model.Database;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Core.Repository
{
    public class SummonerRepository : ISummonerRepository
    {
        private readonly ILogger<SummonerRepository> _logger;
        private readonly IMongoDatabase _mongoDB;
        private readonly IMongoCollection<Summoner> _mongoCollection;
        private readonly IDateService _dateService;

        public SummonerRepository(ILogger<SummonerRepository> logger, IMongoDatabase mongoDB, IDateService dateService)
        {
            _logger = logger;
            _mongoDB = mongoDB;
            _dateService = dateService;

            _mongoCollection = _mongoDB.GetCollection<Summoner>("summonerschemas");
        }

        public virtual async Task<Summoner> findOneSummonerWithFilter(FilterDefinition<Summoner> filterDefinition)
        {
            var result = await _mongoCollection.FindAsync(filterDefinition);

            return result.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<Summoner>> findSummonerWithFilter(FilterDefinition<Summoner> filterDefinition)
        {
            var result = await _mongoCollection.FindAsync(filterDefinition);

            return result.ToEnumerable();
        }

        public virtual async Task replaceSummoner(Summoner summoner)
        {
            summoner.updatedAt = summoner.updatedAt = _dateService!.generateUnixTimeStampMilliseconds();

            var builder = Builders<Summoner>.Filter;

            var filter = builder.Eq(s => s.summonerId, summoner.summonerId);

            var result = await _mongoCollection.ReplaceOneAsync(filter, summoner, new ReplaceOptions()
            {
                IsUpsert = true
            });
        }

        public virtual async Task updateSummoner(FilterDefinition<Summoner> filterDefiniton, UpdateDefinition<Summoner> updateDefinition)
        {
            var result = await _mongoCollection.UpdateManyAsync(filterDefiniton, updateDefinition);
        }

        public virtual async Task createSummoner(Summoner summoner)
        {
            summoner.updatedAt = _dateService.generateUnixTimeStampMilliseconds();

            await _mongoCollection.InsertOneAsync(summoner);
        }
    }
}