using AutoMapper;
using RecaudacionApiComprobanteEmisor.Application.Command.Dtos;
using RecaudacionApiComprobanteEmisor.Domain;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<ComprobanteEmisorFormDto, ComprobanteEmisor>();
            CreateMap<ComprobanteEmisor, ComprobanteEmisorFormDto>();
        }
    }
}