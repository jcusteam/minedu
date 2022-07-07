using AutoMapper;
using RecaudacionApiComprobanteRetencion.Application.Command.Dtos;
using RecaudacionApiComprobanteRetencion.Domain;

namespace RecaudacionApiComprobanteRetencion.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<ComprobanteRetencionFormDto, ComprobanteRetencion>();
            CreateMap<ComprobanteRetencionDetalleFormDto, ComprobanteRetencionDetalle>();
            CreateMap<ComprobanteRetencion, ComprobanteRetencionFormDto>();
        }
    }
}