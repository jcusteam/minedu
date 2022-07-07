using System.Collections.Generic;

namespace RecaudacionApiDepositoBanco.Application.Command.Dtos
{
    public class DepositoBancoFileDto
    {
        public List<DepositoBancoDetalleFormDto> Detalles { get; set; }
        public string UsuarioCreador { get; set; }

        public DepositoBancoFileDto()
        {
            Detalles = new List<DepositoBancoDetalleFormDto>();
        }
    }
}
