using AutoMapper;
using RecaudacionApiEstado.Application.Query.Dtos;
using RecaudacionApiEstado.Domain;
using RecaudacionUtils;

namespace RecaudacionApiEstado.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<Estado, EstadoDto>();
            CreateMap<EstadoFilterDto, EstadoFilter>();
            CreateMap<Pagination<Estado>, Pagination<EstadoDto>>();
        }
    }
}
