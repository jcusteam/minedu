using AutoMapper;
using RecaudacionApiReciboIngreso.Application.Query.Dtos;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<ReciboIngreso, ReciboIngresoDto>();
            CreateMap<ReciboIngresoFilterDto, ReciboIngresoFilter>();
            CreateMap<ReciboIngresoDetalle, ReciboIngresoDetalleDto>();
            CreateMap<Pagination<ReciboIngreso>, Pagination<ReciboIngresoDto>>();
        }
    }
}
