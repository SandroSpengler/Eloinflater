using Core.Enum;
using Core.Model.Riot_Games;
using Namespace;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRiotGamesApi
    {
        [Header("X-Riot-Token")]
        AuthenticationHeaderValue Authorization { get; set; }

        [Get("lol/league/v4/{league}leagues/by-queue/{queue}")]
        Task<RGApiSummonerByLeague> GetSummonerByLeague([Path] string league, [Path] string queue);

        [Get("lol/summoner/v4/summoners/by-name/{summonerName}")]
        Task<RGApiSummoner> GetSummonerByName([Path] string summonerName);

        [Get("lol/summoner/v4/summoners/{summonerId}")]
        Task<RGApiSummoner> GetSummonerBySummonerId([Path] string summonerId);
    }
}