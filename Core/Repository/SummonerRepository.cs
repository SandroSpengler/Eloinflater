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
        private readonly IDateService _dateService;

        public SummonerRepository()
        {
        }

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
            var builder = Builders<Summoner>.Filter;

            var filter = builder.Eq("_id", summoner._id);

            var result = await _mongoCollection.ReplaceOneAsync(filter, summoner);
        }

        public virtual async Task updateSummoner(FilterDefinition<Summoner> filterDefiniton, UpdateDefinition<Summoner> updateDefinition)
        {
            var result = await _mongoCollection.UpdateManyAsync(filterDefiniton, updateDefinition);
        }

        public virtual async Task createSummoner(Summoner summoner)
        {
            summoner.createdAt = _dateService.generateUnixTimeStampMilliseconds();

            await _mongoCollection.InsertOneAsync(summoner);
        }
    }
}