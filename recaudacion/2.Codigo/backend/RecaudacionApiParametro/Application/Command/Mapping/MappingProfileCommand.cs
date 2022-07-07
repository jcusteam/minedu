using AutoMapper;
using RecaudacionApiParametro.Application.Command.Dtos;
using RecaudacionApiParametro.Domain;

namespace RecaudacionApiParametro.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<ParametroFormDto, Parametro>();
            CreateMap<Parametro, ParametroFormDto>();
        }
    }
}