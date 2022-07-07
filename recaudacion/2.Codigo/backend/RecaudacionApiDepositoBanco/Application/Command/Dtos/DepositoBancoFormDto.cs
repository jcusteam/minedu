using System;
using System.Collections.Generic;

namespace RecaudacionApiDepositoBanco.Application.Command.Dtos
{
    public class DepositoBancoFormDto
    {
        public int DepositoBancoId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int BancoId { get; set; }
        public int CuentaCorrienteId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public decimal Importe { get; set; }
        public DateTime FechaDeposito { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NombreArchivo { get; set; }
        public int Cantidad { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<DepositoBancoDetalleFormDto> DepositoBancoDetalle { get; set; }

        public DepositoBancoFormDto()
        {
            DepositoBancoDetalle = new List<DepositoBancoDetalleFormDto>();
        }
    }
}
