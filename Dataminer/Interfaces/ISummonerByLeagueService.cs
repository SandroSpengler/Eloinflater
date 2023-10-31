namespace Dataminer.Interfaces
{
    public interface ISummonerByLeagueService
    {
        bool isUpdateable(long lastUpdate, long updateInterval);

        Task validateSummonerByLeague();
    }
}