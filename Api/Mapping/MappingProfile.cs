using AutoMapper;
using Core.Model.Database;

namespace Namespace;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Summoner, SummonerDTO>()
        .AfterMap((sbl, target) =>
        {
            if (target.rankSolo is null) return;
            if (sbl.rankSolo is null) return;

            target.rankSolo = sbl.rankSolo!.ToLower();
        })
        .ReverseMap();

        CreateMap<RGApiSummoner, SummonerDTO>()
        .ReverseMap();

        CreateMap<RGApiSummoner, Summoner>()
        .ForMember(summoner => summoner._id, opt => opt.Ignore())
        .ForMember(summoner => summoner.__v, opt => opt.Ignore())
        .ForMember(summoner => summoner.summonerId, opt => opt.MapFrom(src => src.id))
        .ReverseMap();
    }
}
