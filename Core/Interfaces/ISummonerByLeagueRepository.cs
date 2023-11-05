using Core.Model;
using Core.Model.Database;
using MongoDB.Driver;

namespace Core.Interfaces
{
    public interface ISummonerByLeagueRepository
    {
        Task<IEnumerable<SummonerByLeague>> findSummonerByLeagueWithFilter(FilterDefinition<SummonerByLeague> filterDefinition);

        Task updateSummonerByLeague(SummonerByLeague summoner);
    }
}