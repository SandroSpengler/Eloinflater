using Amazon.Runtime.Internal.Util;
using Core.Interfaces;
using Core.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository
{
    public class SummonerByLeagueRepository : ISummonerByLeagueRepository
    {

        private readonly ILogger<SummonerByLeagueRepository> _logger;

        private readonly IMongoDatabase _mongoDB;
        private readonly IMongoCollection<SummonerByLeague> _mongoCollection;

        public SummonerByLeagueRepository(ILogger<SummonerByLeagueRepository> logger, IMongoDatabase mongoDB)
        {
            _logger = logger;
            _mongoDB = mongoDB;

            _mongoCollection = _mongoDB.GetCollection<SummonerByLeague>("summonerbyleagueschemas");
        }

        public async Task<IEnumerable<SummonerByLeague>> findSummonerByLeagueWithFilter(FilterDefinition<SummonerByLeague> filterDefinition)
        {
            var result = await _mongoCollection.FindAsync(filterDefinition);

            return result.ToEnumerable();
        }
    }
}
