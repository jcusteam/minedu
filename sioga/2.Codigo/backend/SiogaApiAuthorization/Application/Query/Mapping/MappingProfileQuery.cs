using AutoMapper;
using SiogaApiAuthorization.Application.Query.Dtos;
using SiogaApiAuthorization.Domain;

namespace SiogaApiAuthorization.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<UsuarioToken, UsuarioInstitucionTokenDto>();
        }
    }
}