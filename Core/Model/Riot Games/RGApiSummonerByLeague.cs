namespace Core.Model.Riot_Games
{
    public class RGApiEntry
    {
        public string summonerId { get; set; }
        public string summonerName { get; set; }
        public int leaguePoints { get; set; }
        public string rank { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public bool veteran { get; set; }
        public bool inactive { get; set; }
        public bool freshBlood { get; set; }
        public bool hotStreak { get; set; }
    }

    public class RGApiSummonerByLeague
    {
        public string tier { get; set; }
        public string leagueId { get; set; }
        public string queue { get; set; }
        public string name { get; set; }
        public List<RGApiEntry> entries { get; set; }
    }
}