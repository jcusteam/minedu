using AutoMapper;
using RecaudacionApiComprobanteRetencion.Application.Query.Dtos;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteRetencion.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<ComprobanteRetencion, ComprobanteRetencionDto>();
            CreateMap<ComprobanteRetencionDetalle, ComprobanteRetencionDetalleDto>();
            CreateMap<ComprobanteRetencionFilterDto, ComprobanteRetencionFilter>();
            CreateMap<Pagination<ComprobanteRetencion>, Pagination<ComprobanteRetencionDto>>();
        }
    }
}
