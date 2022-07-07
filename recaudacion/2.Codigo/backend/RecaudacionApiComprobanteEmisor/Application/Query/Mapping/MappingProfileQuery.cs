using AutoMapper;
using RecaudacionApiComprobanteEmisor.Application.Query.Dtos;
using RecaudacionApiComprobanteEmisor.Domain;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteEmisor.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<ComprobanteEmisor, ComprobanteEmisorDto>();
            CreateMap<ComprobanteEmisorFilterDto, ComprobanteEmisorFilter>();
            CreateMap<Pagination<ComprobanteEmisor>, Pagination<ComprobanteEmisorDto>>().ReverseMap();
        }
    }
}
