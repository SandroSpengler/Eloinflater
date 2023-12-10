using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Model.Database
{
    public class Summoner
    {
        [BsonElement("_id")]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public int profileIconId { get; set; }
        public long revisionDate { get; set; }
        public int summonerLevel { get; set; }
        public int leaguePoints { get; set; }
        public string? rank { get; set; }
        public string? rankSolo { get; set; }
        public string? flexSolo { get; set; }
        public string? flextt { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public bool veteran { get; set; }
        public bool inactive { get; set; }
        public bool freshBlood { get; set; }
        public bool hotStreak { get; set; }
        public List<string>? inflatedMatchList { get; set; }
        public int exhaustCount { get; set; }
        public int exhaustCastCount { get; set; }
        public int tabisCount { get; set; }
        public int zhonaysCount { get; set; }
        public int zhonaysCastCount { get; set; }
        public string? summonerId { get; set; }
        public string? accountId { get; set; }
        public string? puuid { get; set; }
        public string? name { get; set; }
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
        public int __v { get; set; }
        public List<string>? uninflatedMatchList { get; set; }
        public long lastMatchUpdate { get; set; }
        public int outstandingMatches { get; set; }
    }
}