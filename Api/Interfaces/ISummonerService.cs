namespace Namespace;
public interface ISummonerService
{
    Task<SummonerDTO> GetSummonerByName(string summonerName);

    /// <summary>
    /// Determines if a SummonerName like the searchString exsists
    /// </summary>
    /// <param name="summonerName">String that the name should contain</param>
    /// <returns></returns>
    Task<IEnumerable<SummonerDTO>> GetSummonerAutoComplete(string summonerName);

    /// <summary>
    /// Determines if Summoner can be found via puuid otherwise creates the summoner 
    /// </summary>
    /// <param name="summonerName"></param>
    /// <returns></returns>
    Task<SummonerDTO> GetSummonerFromRGApi(string summonerName);
}
