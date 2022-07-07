using AutoMapper;
using SiogaApiPide.Application.Query.Dtos;
using SiogaApiPide.Domain;

namespace SiogaApiPide.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<ReniecSubvencionDto, Reniec>().ReverseMap();
            CreateMap<SunatSubvencionDto, Sunat>().ReverseMap();
            CreateMap<SunatRepresentanteSubvencionDto, MultiRef>().ReverseMap();

            CreateMap<ReniecDto, Reniec>().ReverseMap();
            CreateMap<MigracionDto, Migracion>().ReverseMap();
            CreateMap<SunatDto, Sunat>().ReverseMap();
            CreateMap<SunatRepresentanteDto, MultiRef>().ReverseMap();
        }
    }
}
