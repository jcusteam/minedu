using AutoMapper;
using RecaudacionApiEstado.Application.Command.Dtos;
using RecaudacionApiEstado.Domain;

namespace RecaudacionApiEstado.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<EstadoFormDto, Estado>();
            CreateMap<Estado, EstadoFormDto>();
        }
    }
}