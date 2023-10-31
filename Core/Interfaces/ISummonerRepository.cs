using Core.Model.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Interfaces
{
    public interface ISummonerRepository
    {
        Task<IEnumerable<Summoner>> findSummonerWithFilter(FilterDefinition<Summoner> filterDefinition);

        Task<Summoner> findOneSummonerWithFilter(FilterDefinition<Summoner> filterDefinition);

        Task updateSummoner(Summoner summoner);

        Task createSummoner(Summoner summoner);
    }
}