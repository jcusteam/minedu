using AutoMapper;
using RecaudacionApiRegistroLinea.Application.Query.Dtos;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<RegistroLinea, RegistroLineaDto>();
            CreateMap<RegistroLineaFilterDto, RegistroLineaFilter>();
            CreateMap<RegistroLineaDetalle, RegistroLineaDetalleDto>();
            CreateMap<Pagination<RegistroLinea>, Pagination<RegistroLineaDto>>();
        }
    }
}
