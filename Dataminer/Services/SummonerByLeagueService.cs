using Core.Interfaces;
using Core.Model;
using Core.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataminer.Services
{
    public class SummonerByLeagueService
    {

        private readonly ILogger<SummonerByLeagueService> _logger;
        private readonly ISummonerByLeagueRepository _repository;

        public SummonerByLeagueService(ILogger<SummonerByLeagueService> logger, ISummonerByLeagueRepository summonerByLeagueRepository)
        {
            _logger = logger;
            _repository = summonerByLeagueRepository;
        }

        public async Task validateSummonerByLeague()
        {
            var builder = Builders<SummonerByLeague>.Filter.Empty;

            var summonerByLeague = (await _repository.findSummonerByLeagueWithFilter(builder)).ToList();

            _logger.LogInformation($"SummonerByLeagueCount {summonerByLeague.Count()}");

            foreach (SummonerByLeague sbl in summonerByLeague)
            {
                _logger.LogInformation($"SummonerByLeagueCollectionName {sbl.tier}");
            }
        }
    }
}
