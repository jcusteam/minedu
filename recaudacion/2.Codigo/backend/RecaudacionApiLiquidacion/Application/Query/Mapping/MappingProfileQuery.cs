using AutoMapper;
using RecaudacionApiLiquidacion.Application.Query.Dtos;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<Liquidacion, LiquidacionDto>();
            CreateMap<LiquidacionFilterDto, LiquidacionFilter>();
            CreateMap<LiquidacionDetalle, LiquidacionDetalleDto>();
            CreateMap<Pagination<Liquidacion>, Pagination<LiquidacionDto>>();
        }
    }
}
