using AutoMapper;
using RecaudacionApiTipoDocumento.Application.Command.Dtos;
using RecaudacionApiTipoDocumento.Domain;

namespace RecaudacionApiTipoDocumento.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<TipoDocumentoFormDto, TipoDocumento>();
            CreateMap<TipoDocumento, TipoDocumentoFormDto>();
        }
    }
}