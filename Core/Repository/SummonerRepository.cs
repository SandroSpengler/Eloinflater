using Core.Interfaces;
using Core.Model;
using Core.Model.Database;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository
{
    public class SummonerRepository : ISummonerRepository
    {
        private readonly ILogger<SummonerRepository> _logger;

        private readonly IMongoDatabase _mongoDB;
        private readonly IMongoCollection<Summoner> _mongoCollection;

        public SummonerRepository()
        {
        }

        public SummonerRepository(ILogger<SummonerRepository> logger, IMongoDatabase mongoDB)
        {
            _logger = logger;
            _mongoDB = mongoDB;

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

        public virtual async Task updateSummoner(Summoner summoner)
        {
            var builder = Builders<Summoner>.Filter;

            var filter = builder.Eq("_id", summoner);

            var result = await _mongoCollection.ReplaceOneAsync(filter, summoner);
        }

        public virtual async Task createSummoner(Summoner summoner)
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long currentDate = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond) * 1000;

            summoner.createdAt = currentDate;

            await _mongoCollection.InsertOneAsync(summoner);
        }
    }
}