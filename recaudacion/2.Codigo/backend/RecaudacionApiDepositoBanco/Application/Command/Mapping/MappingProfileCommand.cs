using AutoMapper;
using RecaudacionApiDepositoBanco.Application.Command.Dtos;
using RecaudacionApiDepositoBanco.Domain;

namespace RecaudacionApiDepositoBanco.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<DepositoBancoFormDto, DepositoBanco>();
            CreateMap<DepositoBanco, DepositoBancoFormDto>();
            CreateMap<DepositoBancoDetalleFormDto, DepositoBancoDetalle>();
            CreateMap<DepositoBancoDetalle, DepositoBancoDetalleFormDto>();
        }
    }
}