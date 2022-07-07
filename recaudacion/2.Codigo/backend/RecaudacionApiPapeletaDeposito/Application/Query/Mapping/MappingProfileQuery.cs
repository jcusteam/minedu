using AutoMapper;
using RecaudacionApiPapeletaDeposito.Application.Query.Dtos;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<PapeletaDeposito, PapeletaDepositoDto>();
            CreateMap<PapeletaDepositoFilterDto, PapeletaDepositoFilter>();
            CreateMap<PapeletaDepositoDetalle, PapeletaDepositoDetalleDto>();
            CreateMap<Pagination<PapeletaDeposito>, Pagination<PapeletaDepositoDto>>();
        }
    }
}
