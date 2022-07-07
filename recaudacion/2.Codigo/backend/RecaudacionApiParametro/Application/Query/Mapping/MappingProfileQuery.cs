using AutoMapper;
using RecaudacionApiParametro.Application.Query.Dtos;
using RecaudacionApiParametro.Domain;

namespace RecaudacionApiParametro.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<Parametro, ParametroDto>();
            CreateMap<ParametroFilterDto, ParametroFilter>();
        }
    }
}