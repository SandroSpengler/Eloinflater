using Core.Model.Riot_Games;
using Core.Model;

namespace Dataminer.Interfaces
{
    public interface IMiningService
    {
        /// <summary>
        /// Checks if an update has been done before the last updateInterval
        /// </summary>
        /// <param name="lastUpdate">time of the last update as a UNIX-Timestamp in MS</param>
        /// <param name="updateInterval">how often an update can occur as a UNIX-Timestamp</param>
        /// <returns></returns>
        bool isUpdateable(long lastUpdate, long updateInterval);

        /// <summary>
        /// Refreshes all Summoners in Master and above every 12H
        /// </summary>
        /// <returns></returns>
        Task refreshSummonerByLeague();

        /// <summary>
        /// Takes the Riot SummonerByLeague and updates the SummonerByLeague in DB
        /// </summary>
        /// <param name="sblDB">Current SummonerByLeague in DB</param>
        /// <param name="sblRiot">New SummonerByLeague from RiotGamesApi</param>
        /// <returns></returns>
        Task updateAllSummonerByLeagueEntries(SummonerByLeague sblDB, RGApiSummonerByLeague sblRiot);
        Task validateAllSummoners();
    }
}