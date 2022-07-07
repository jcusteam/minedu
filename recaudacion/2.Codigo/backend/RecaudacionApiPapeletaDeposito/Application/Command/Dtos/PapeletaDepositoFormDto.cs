using System;
using System.Collections.Generic;

namespace RecaudacionApiPapeletaDeposito.Application.Command.Dtos
{
    public class PapeletaDepositoFormDto
    {
        public int PapeletaDepositoId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int BancoId { get; set; }
        public int CuentaCorrienteId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<PapeletaDepositoDetalleFormDto> PapeletaDepositoDetalle { get; set; }

        public PapeletaDepositoFormDto()
        {
            PapeletaDepositoDetalle = new List<PapeletaDepositoDetalleFormDto>();
        }
    }
}