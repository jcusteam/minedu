using AutoMapper;
using RecaudacionApiIngresoPecosa.Application.Query.Dtos;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<IngresoPecosa, IngresoPecosaDto>();
            CreateMap<IngresoPecosaFilterDto, IngresoPecosaFilter>();
            CreateMap<IngresoPecosaDetalle, IngresoPecosaDetalleDto>();
            CreateMap<Pagination<IngresoPecosa>, Pagination<IngresoPecosaDto>>();

            CreateMap<CatalogoBienFilterDto, CatalogoBienFilter>();
            CreateMap<CatalogoBien, CatalogoBienDto>();
            CreateMap<Pagination<CatalogoBien>, Pagination<CatalogoBienDto>>();
        }
    }
}
