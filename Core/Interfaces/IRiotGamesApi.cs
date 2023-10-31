using Core.Enum;
using Core.Model.Riot_Games;
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
        Task<RGApiSummonerByLeague> GetSummonerByLeague([Path] League league, [Path] Queue queue);
    }
}