using AutoMapper;
using RecaudacionApiTipoDocumento.Application.Query.Dtos;
using RecaudacionApiTipoDocumento.Domain;
using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<TipoDocumento, TipoDocumentoDto>();
            CreateMap<TipoDocumentoFilterDto, TipoDocumentoFilter>();
            CreateMap<Pagination<TipoDocumento>, Pagination<TipoDocumentoDto>>();
        }
    }
}
