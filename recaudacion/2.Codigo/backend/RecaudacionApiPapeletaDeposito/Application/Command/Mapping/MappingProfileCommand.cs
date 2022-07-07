using AutoMapper;
using RecaudacionApiPapeletaDeposito.Application.Command.Dtos;
using RecaudacionApiPapeletaDeposito.Domain;

namespace RecaudacionApiPapeletaDeposito.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<PapeletaDepositoFormDto, PapeletaDeposito>();
            CreateMap<PapeletaDeposito, PapeletaDepositoFormDto>();
             CreateMap<PapeletaDepositoDetalleFormDto, PapeletaDepositoDetalle>();
        }
    }
}