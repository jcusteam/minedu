using AutoMapper;
using RecaudacionApiLiquidacion.Application.Command.Dtos;
using RecaudacionApiLiquidacion.Domain;

namespace RecaudacionApiLiquidacion.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<LiquidacionFormDto, Liquidacion>();
            CreateMap<Liquidacion, LiquidacionFormDto>();
        }
    }
}