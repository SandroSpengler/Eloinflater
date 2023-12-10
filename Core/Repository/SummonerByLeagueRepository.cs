using Core.Interfaces;
using Core.Model;
using Core.Model.Database;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Core.Repository
{
    public class SummonerByLeagueRepository : ISummonerByLeagueRepository
    {
        private readonly ILogger<SummonerByLeagueRepository> _logger;
        private readonly IMongoDatabase _mongoDB;
        private readonly IMongoCollection<SummonerByLeague> _mongoCollection;
        private readonly IDateService _dateService;

        public SummonerByLeagueRepository(ILogger<SummonerByLeagueRepository> logger, IMongoDatabase mongoDB, IDateService dateService)
        {
            _logger = logger;
            _mongoDB = mongoDB;

            _mongoCollection = _mongoDB.GetCollection<SummonerByLeague>("summonerbyleagueschemas");
            _dateService = dateService;
        }

        public virtual async Task<IEnumerable<SummonerByLeague>> findSummonerByLeagueWithFilter(FilterDefinition<SummonerByLeague> filterDefinition)
        {
            var result = await _mongoCollection.FindAsync(filterDefinition);

            return result.ToEnumerable();
        }

        public virtual async Task replaceSummonerByLeague(SummonerByLeague summonerByLeague)
        {
            var builder = Builders<SummonerByLeague>.Filter;

            var filter = builder.Eq(sbl => sbl._id, summonerByLeague._id);

            summonerByLeague.updatedAt = _dateService.generateUnixTimeStampMilliseconds();

            var result = await _mongoCollection.ReplaceOneAsync(filter, summonerByLeague);
        }
    }
}