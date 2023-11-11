using Core.Enum;
using Core.Interfaces;
using Core.Model;
using Core.Model.Database;
using Core.Model.Riot_Games;
using Dataminer.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection.Metadata;

namespace Dataminer.Services
{
    public class SummonerByLeagueService : ISummonerByLeagueService
    {
        private readonly ILogger<SummonerByLeagueService> _logger;
        private readonly ISummonerByLeagueRepository _sblRepository;
        private readonly ISummonerRepository _summonerRepository;
        private readonly IRiotGamesApi _riotGamesApi;
        private readonly FilterDefinitionBuilder<Summoner> _summonerFilterBuilder;
        private readonly UpdateDefinitionBuilder<Summoner> _summonerUpdateBuilder;
        private readonly FilterDefinitionBuilder<SummonerByLeague> _sblFilterBuilder;
        private readonly UpdateDefinitionBuilder<SummonerByLeague> _sblUpdateBuilder;

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
            _summonerFilterBuilder = Builders<Summoner>.Filter;
            _summonerUpdateBuilder = Builders<Summoner>.Update;
            _sblFilterBuilder = Builders<SummonerByLeague>.Filter;
            _sblUpdateBuilder = Builders<SummonerByLeague>.Update;
        }

        public async Task validateSummonerByLeague()
        {
            long UpdateInterval_12H = 12 * 60 * 60 * 1000;
            var summonerByLeagueDB = (await _sblRepository.findSummonerByLeagueWithFilter(_sblFilterBuilder.Empty))
                .ToList();

            foreach (SummonerByLeague sblDB in summonerByLeagueDB)
            {
                _logger.LogInformation("Updating SummonerByLeague {0}", sblDB.tier);

                if (!isUpdateable(sblDB.updatedAt, UpdateInterval_12H)) continue;

                try
                {
                    // ToDo
                    // reset all with rank in DB
                    // find summonerWithFilter => UpdateSummonerByFilter
                    await _summonerRepository.updateSummoner(
                        _summonerFilterBuilder.Eq(
                            s => s.rankSolo, sblDB.tier.ToString()
                        ),
                        _summonerUpdateBuilder.Set(s => s.rankSolo, "")
                    );

                    RGApiSummonerByLeague riotSummonerByLeague = await _riotGamesApi
                        .GetSummonerByLeague(
                            sblDB.tier,
                            Queue.RANKED_SOLO_5x5
                        );

                    await updateAllSummonerByLeagueEntries(sblDB, riotSummonerByLeague);

                    // ToDo Update SummonerByLeague
                    // Map riotSummonerByLeague -> sblDB
                    //await _sblRepository.updateSummoner(sblDB);

                    await _sblRepository.replaceSummonerByLeague(sblDB);

                    _logger.LogInformation("Finished updating SummonerByLeague {0}", sblDB.tier);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed Updating SummonerByLeague: {0}", ex.Message);
                }
            }
        }

        public async Task updateAllSummonerByLeagueEntries(SummonerByLeague sblDB, RGApiSummonerByLeague sblRiot)
        {
            int index = 0;
            int lastPercent = 0;
            foreach (RGEntry entry in sblRiot.entries)
            {
                var filter = _summonerFilterBuilder.Eq(s => s._id, entry.summonerId);

                Summoner summonerDB = await _summonerRepository.findOneSummonerWithFilter(filter);

                double percentage = ((double)index / sblRiot.entries.Count) * 100;

                Summoner summoner = new Summoner();

                summoner._id = entry.summonerId;
                summoner.rankSolo = sblDB.tier.ToString();
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
                        sblRiot.entries.Count,
                        index == 0 ? 0 : (int)percentage
                    );

                    index++;

                    continue;
                }

                await _summonerRepository.replaceSummoner(summoner);

                if ((int)percentage >= lastPercent + 5)
                {
                    _logger.LogInformation("Updated - {0}/{1} - {2}%",
                        index,
                        sblRiot.entries.Count,
                        index == 0 ? 0 : (int)percentage
                    );
                    lastPercent = (int)percentage;
                }
                index++;
            }
        }

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