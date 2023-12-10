using AutoMapper;
using Core.Model;
using Core.Model.Database;
using Core.Model.Riot_Games;
namespace Namespace;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SummonerByLeague, RGApiSummonerByLeague>()
        .AfterMap((sbl, target) =>
        {
            target.tier = sbl.tier.ToLower();
        })
        .ReverseMap();

        CreateMap<Entry, RGApiEntry>().ReverseMap();

        CreateMap<RGApiSummoner, Summoner>()
        .ForMember(summoner => summoner.summonerId, opt => opt.MapFrom(src => src.id))
        .ReverseMap();
    }
}
