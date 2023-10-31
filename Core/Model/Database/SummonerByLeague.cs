using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace Core.Model
{
    public class Entry
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
        public object createdAt { get; set; }
        public object updatedAt { get; set; }
    }

    public class SummonerByLeague
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public List<Entry> entries { get; set; }
        public Enum.League tier { get; set; }
        public string leagueId { get; set; }
        public Enum.Queue queue { get; set; }
        public string name { get; set; }
        public long createdAt { get; set; }
        public long updatedAt { get; set; }
        public int __v { get; set; }
    }
}