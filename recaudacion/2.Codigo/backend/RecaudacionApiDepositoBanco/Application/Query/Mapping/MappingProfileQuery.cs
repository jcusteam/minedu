using AutoMapper;
using RecaudacionApiDepositoBanco.Application.Query.Dtos;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<DepositoBanco, DepositoBancoDto>();
            CreateMap<DepositoBancoDetalle, DepositoBancoDetalleDto>();
            CreateMap<DepositoBancoFilterDto, DepositoBancoFilter>();
            CreateMap<Pagination<DepositoBanco>, Pagination<DepositoBancoDto>>();
        }
    }
}
