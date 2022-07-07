using AutoMapper;
using RecaudacionApiRegistroLinea.Application.Command.Dtos;
using RecaudacionApiRegistroLinea.Domain;

namespace RecaudacionApiRegistroLinea.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<RegistroLineaFormDto, RegistroLinea>();
            CreateMap<RegistroLinea, RegistroLineaFormDto>();
            CreateMap<RegistroLineaDetalleFormDto, RegistroLineaDetalle>();
        }
    }
}