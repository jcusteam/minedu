using AutoMapper;
using RecaudacionApiIngresoPecosa.Application.Command.Dtos;
using RecaudacionApiIngresoPecosa.Domain;

namespace RecaudacionApiIngresoPecosa.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<IngresoPecosaFormDto, IngresoPecosa>();
            CreateMap<IngresoPecosa, IngresoPecosaFormDto>();
            CreateMap<IngresoPecosaDetalleFormDto, IngresoPecosaDetalle>();
        }
    }
}