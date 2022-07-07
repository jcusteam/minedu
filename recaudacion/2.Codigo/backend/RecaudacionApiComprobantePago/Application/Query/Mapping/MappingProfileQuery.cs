using AutoMapper;
using RecaudacionApiComprobantePago.Application.Query.Dtos;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<ComprobantePago, ComprobantePagoDto>();
            CreateMap<ComprobantePagoDetalle, ComprobantePagoDetalleDto>();
            CreateMap<ComprobantePagoFilterDto, ComprobantePagoFilter>();
            CreateMap<Pagination<ComprobantePago>, Pagination<ComprobantePagoDto>>();
        }
    }
}
