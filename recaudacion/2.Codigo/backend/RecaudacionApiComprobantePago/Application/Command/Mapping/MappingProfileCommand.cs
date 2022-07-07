using AutoMapper;
using RecaudacionApiComprobantePago.Application.Command.Dtos;
using RecaudacionApiComprobantePago.Domain;

namespace RecaudacionApiComprobantePago.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<ComprobantePagoFormDto, ComprobantePago>();
            CreateMap<ComprobantePago, ComprobantePagoFormDto>();
            CreateMap<ComprobantePagoDetalleFormDto, ComprobantePagoDetalle>();
        }
    }
}
