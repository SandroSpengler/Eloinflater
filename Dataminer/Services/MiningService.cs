using AutoMapper;
using Core.Enum;
using Core.Interfaces;
using Core.Model;
using Core.Model.Database;
using Core.Model.Riot_Games;
using Dataminer.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Reflection.Metadata;

namespace Dataminer.Services
{
    public class MiningService : IMiningService
    {
        private readonly ILogger<MiningService> _logger;
        private readonly IMapper _mapper;
        private readonly ISummonerByLeagueRepository _sblRepository;
        private readonly ISummonerRepository _summonerRepository;
        private readonly IRiotGamesApi _riotGamesApi;
        private readonly FilterDefinitionBuilder<Summoner> _summonerFilterBuilder;
        private readonly UpdateDefinitionBuilder<Summoner> _summonerUpdateBuilder;
        private readonly FilterDefinitionBuilder<SummonerByLeague> _sblFilterBuilder;
        private readonly UpdateDefinitionBuilder<SummonerByLeague> _sblUpdateBuilder;

        public MiningService(
            ILogger<MiningService> logger,
            IMapper mapper,
            ISummonerByLeagueRepository summonerByLeagueRepository,
            ISummonerRepository summonerRepository,
            IRiotGamesApi riotGamesApi
        )
        {
            _logger = logger;
            _mapper = mapper;
            _sblRepository = summonerByLeagueRepository;
            _summonerRepository = summonerRepository;
            _riotGamesApi = riotGamesApi;
            _summonerFilterBuilder = Builders<Summoner>.Filter;
            _summonerUpdateBuilder = Builders<Summoner>.Update;
            _sblFilterBuilder = Builders<SummonerByLeague>.Filter;
            _sblUpdateBuilder = Builders<SummonerByLeague>.Update;
        }

        public async Task validateAllSummoners()
        {
            try
            {
                var filter = _summonerFilterBuilder.Where(summoner =>
                    String.IsNullOrEmpty(summoner.puuid) ||
                    String.IsNullOrEmpty(summoner.accountId)
                );

                var invalidSummoner = (await _summonerRepository.findSummonerWithFilter(filter)).ToList();

                _logger.LogInformation("Invalid Summoners: {0}",
                        invalidSummoner.Count()
                    );

                int index = 0;

                foreach (Summoner summoner in invalidSummoner)
                {

                    _logger.LogInformation("Validating {0}/{1} - {2}",
                        index,
                        invalidSummoner.Count(),
                        summoner.name
                );

                    var rgSummoner = await _riotGamesApi.GetSummonerBySummonerId(summoner.summonerId);

                    var mappedSummoner = _mapper.Map<Summoner>(rgSummoner);

                    await _summonerRepository.replaceSummoner(mappedSummoner);

                    index++;
                }
            }
            catch (RestEase.ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    _logger.LogError("validateAllSummoners | Exiting due to invalid API_Key | 403");
                    Environment.Exit(1);
                }

                _logger.LogError("validateAllSummoners | Failed Updating SummonerByLeague: {0}", ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("validateAllSummoners | Failed Updating SummonerByLeague: {0}", ex.Message);
            }
        }

        public async Task refreshSummonerByLeague()
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
                    RGApiSummonerByLeague riotSummonerByLeague = await _riotGamesApi
                        .GetSummonerByLeague(
                            sblDB.tier.ToLower(),
                            Queue.RANKED_SOLO_5x5.ToString()
                        );

                    await _summonerRepository.updateSummoner(
                        _summonerFilterBuilder.Eq(
                            s => s.rankSolo, sblDB.tier.ToString()
                        ),
                        _summonerUpdateBuilder
                            .Set(s => s.rankSolo, "")
                    );

                    await updateAllSummonerByLeagueEntries(sblDB, riotSummonerByLeague);

                    var updatedSbl = _mapper.Map<SummonerByLeague>(riotSummonerByLeague);

                    await _sblRepository.replaceSummonerByLeague(sblDB);

                    _logger.LogInformation("Finished updating SummonerByLeague {0}", sblDB.tier);
                }
                catch (RestEase.ApiException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.Forbidden)
                    {
                        _logger.LogError("refreshSummonerByLeague | Exiting due to invalid API_Key | 403");
                        Environment.Exit(1);
                    }

                    _logger.LogError("refreshSummonerByLeague | Failed Updating SummonerByLeague: {0}", ex.Message);

                }
                catch (Exception ex)
                {
                    _logger.LogError("refreshSummonerByLeague | Failed Updating SummonerByLeague: {0}", ex.Message);
                }
            }
        }

        public async Task updateAllSummonerByLeagueEntries(SummonerByLeague sblDB, RGApiSummonerByLeague sblRiot)
        {
            int index = 0;
            int lastPercent = 0;
            foreach (RGApiEntry entry in sblRiot.entries)
            {
                var filter = _summonerFilterBuilder.Eq(s => s.summonerId, entry.summonerId);

                Summoner summonerDB = await _summonerRepository.findOneSummonerWithFilter(filter);

                double percentage = ((double)index / sblRiot.entries.Count) * 100;

                Summoner summoner = new Summoner();

                summoner.summonerId = entry.summonerId;
                summoner.rankSolo = sblDB.tier;
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