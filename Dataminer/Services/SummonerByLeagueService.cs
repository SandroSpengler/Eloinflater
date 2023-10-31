using Core.Enum;
using Core.Interfaces;
using Core.Model;
using Core.Model.Database;
using Core.Model.Riot_Games;
using Dataminer.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dataminer.Services
{
    public class SummonerByLeagueService : ISummonerByLeagueService
    {
        private readonly ILogger<SummonerByLeagueService> _logger;
        private readonly ISummonerByLeagueRepository _sblRepository;
        private readonly ISummonerRepository _summonerRepository;
        private readonly IRiotGamesApi _riotGamesApi;

        public SummonerByLeagueService(
            ILogger<SummonerByLeagueService> logger,
            ISummonerByLeagueRepository summonerByLeagueRepository,
            ISummonerRepository summonerRepository,
            IRiotGamesApi riotGamesApi
        )
        {
            _logger = logger;
            _sblRepository = summonerByLeagueRepository;
            _summonerRepository = summonerRepository;
            _riotGamesApi = riotGamesApi;
        }

        /// <summary>
        /// Refreshes all Summoners in Master and above every 12H
        /// </summary>
        /// <returns></returns>
        public async Task validateSummonerByLeague()
        {
            var sblBuilder = Builders<SummonerByLeague>.Filter.Empty;
            var summonerBuilder = Builders<Summoner>.Filter;

            long UpdateInterval_12H = 12 * 60 * 60 * 1000;
            var summonerByLeague = (await _sblRepository.findSummonerByLeagueWithFilter(sblBuilder))
                .ToList();

            foreach (SummonerByLeague sbl in summonerByLeague)
            {
                _logger.LogInformation("Updating SummonerByLeague {0}", sbl.tier);

                if (!isUpdateable(sbl.updatedAt, UpdateInterval_12H)) continue;

                try
                {
                    RGApiSummonerByLeague riotSummonerByLeague = await _riotGamesApi.GetSummonerByLeague(sbl.tier, Queue.RANKED_SOLO_5x5);

                    int index = 0;
                    int lastPercent = 0;
                    foreach (RGEntry entry in riotSummonerByLeague.entries)
                    {
                        var filter = summonerBuilder.Eq(s => s._id, entry.summonerId);

                        Summoner summonerDB = await _summonerRepository.findOneSummonerWithFilter(filter);

                        double percentage = ((double)index / riotSummonerByLeague.entries.Count) * 100;

                        Summoner summoner = new Summoner();

                        summoner._id = entry.summonerId;
                        summoner.rankSolo = sbl.tier.ToString();
                        summoner.name = entry.summonerName;
                        summoner.leaguePoints = entry.leaguePoints;
                        summoner.rank = entry.rank;
                        summoner.wins = entry.wins;
                        summoner.losses = entry.losses;
                        summoner.veteran = entry.veteran;
                        summoner.inactive = entry.inactive;
                        summoner.freshBlood = entry.freshBlood;
                        summoner.hotStreak = entry.hotStreak;

                        if (summonerDB == null)
                        {
                            await _summonerRepository.createSummoner(summoner);

                            _logger.LogInformation("Created - {0}/{1} - {2}%",
                                index,
                                riotSummonerByLeague.entries.Count,
                                index == 0 ? 0 : (int)percentage
                            );

                            index++;

                            continue;
                        }

                        await _summonerRepository.updateSummoner(summoner);

                        if ((int)percentage >= lastPercent + 5)
                        {
                            _logger.LogInformation("Updated - {0}/{1} - {2}%",
                                index,
                                riotSummonerByLeague.entries.Count,
                                index == 0 ? 0 : (int)percentage
                            );
                            lastPercent = (int)percentage;
                        }
                        index++;
                    }

                    // ToDo Update SummonerByLeague

                    _logger.LogInformation("Finished updating SummonerByLeague {0}", sbl.tier);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed Updating SummonerByLeague: {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Checks if an update has been done before the last updateInterval
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