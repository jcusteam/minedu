using AutoMapper;
using RecaudacionApiReciboIngreso.Application.Command.Dtos;
using RecaudacionApiReciboIngreso.Domain;

namespace RecaudacionApiReciboIngreso.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<ReciboIngresoFormDto, ReciboIngreso>();
            CreateMap<ReciboIngreso, ReciboIngresoFormDto>();
            CreateMap<ReciboIngresoDetalleFormDto, ReciboIngresoDetalle>();
        }
    }
}