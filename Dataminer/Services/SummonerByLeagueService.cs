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

            // Check if last update longer than 8 hours ago
            // get new summonerbyleague from riot games
            // overwrite summonerbyleague in db
            // update all summoner in db with new information
        }

        /// <summary>
        /// checks if the last update has been long enough
        /// </summary>
        /// <param name="lastUpdate">time of the last update as a UNIX-Timestamp in MS</param>
        /// <param name="updateInterval">how often an update can occur as a UNIX-Timestamp</param>
        /// <returns></returns>
        public bool isUpdateable(long lastUpdate, long updateInterval)
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long currentDate = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond) * 1000;
            long updateThreshold = currentDate - updateInterval;

            if (lastUpdate < updateThreshold)
            {
                return true;
            }

            return false;
        }
    }
}