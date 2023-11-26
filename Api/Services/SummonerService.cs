using AutoMapper;
using Core.Interfaces;
using Core.Model.Database;
using MongoDB.Driver;

namespace Namespace;
public class SummonerService : ISummonerService
{
    private readonly ILogger<SummonerService> _logger;
    private readonly IMapper _mapper;
    private readonly ISummonerRepository _summonerRepository;
    private readonly IRiotGamesApi _riotGamesApi;

    public SummonerService(
        ILogger<SummonerService> logger,
        IMapper mapper,
        ISummonerRepository summonerRepository,
        IRiotGamesApi riotGamesApi
        )
    {
        _logger = logger;
        _mapper = mapper;
        _summonerRepository = summonerRepository;
        _riotGamesApi = riotGamesApi;
    }

    public async Task<SummonerDTO> GetSummonerByName(string summonerName)
    {
        var builder = Builders<Summoner>.Filter;

        var filter = builder.Eq(summoner => summoner.name, summonerName);

        Summoner summoner = await _summonerRepository.findOneSummonerWithFilter(filter);

        var summonerDTO = _mapper.Map<SummonerDTO>(summoner);

        return summonerDTO;
    }

    public async Task<IEnumerable<SummonerDTO>> GetSummonerAutoComplete(string summonerName)
    {
        var builder = Builders<Summoner>.Filter;

        var filter = builder.Regex(summoner => summoner.name, $"^{summonerName}.*"); ;

        IEnumerable<Summoner> summoners = await _summonerRepository.findSummonerWithFilter(filter);

        var summonerDTO = _mapper.Map<IEnumerable<SummonerDTO>>(summoners);

        return summonerDTO.ToList();
    }


    public async Task<SummonerDTO> GetSummonerFromRGApi(string summonerName)
    {
        var builder = Builders<Summoner>.Filter;

        var updateBuilder = Builders<Summoner>.Update;

        SummonerDTO? summonerDTO = null;

        try
        {
            RGApiSummoner RGSummoner = await _riotGamesApi.GetSummonerByName(summonerName);

            Summoner mappedSummoner = _mapper.Map<Summoner>(RGSummoner);

            var filterSearch = builder.Eq(summoner => summoner.puuid, RGSummoner.puuid);

            Summoner summonerInDB = await _summonerRepository.findOneSummonerWithFilter(filterSearch);

            if (summonerInDB is null)
            {
                await _summonerRepository.createSummoner(mappedSummoner);

                summonerDTO = _mapper.Map<SummonerDTO>(mappedSummoner);

                return summonerDTO;
            }

            var filterUpdate = builder.Eq(summoner => summoner.puuid, summonerInDB.puuid);

            var updateDefinition = updateBuilder.Set(summoner => summoner.name, mappedSummoner.name);

            await _summonerRepository.updateSummoner(filterUpdate, updateDefinition);

            summonerDTO = _mapper.Map<SummonerDTO>(mappedSummoner);

            return summonerDTO;
        }
        catch (RestEase.ApiException apiEx)
        {
            if (apiEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {

                return null!;
            }

            throw apiEx;
        }
    }
}
