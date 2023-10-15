using Core.Model;
using MongoDB.Driver;

namespace Core.Interfaces
{
    public interface ISummonerByLeagueRepository
    {
        Task<IEnumerable<SummonerByLeague>> findSummonerByLeagueWithFilter(FilterDefinition<SummonerByLeague> filterDefinition);
    }
}