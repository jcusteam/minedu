using System;
using System.Collections.Generic;

namespace RecaudacionApiReciboIngreso.Application.Command.Dtos
{
    public class ReciboIngresoFormDto
    {
        public int ReciboIngresoId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoReciboIngresoId { get; set; }
        public int ClienteId { get; set; }
        public int CuentaCorrienteId { get; set; }
        public int FuenteFinanciamientoId { get; set; }
        public int? RegistroLineaId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public DateTime? FechaEmision { get; set; }
        public int TipoCaptacionId { get; set; }
        public int? DepositoBancoDetalleId { get; set; }
        public decimal ImporteTotal { get; set; }
        public string NumeroDeposito { get; set; }
        public DateTime? FechaDeposito { get; set; }
        public string ValidarDeposito { get; set; }
        public string NumeroCheque { get; set; }
        public string NumeroOficio { get; set; }
        public string NumeroComprobantePago { get; set; }
        public string ExpedienteSiaf { get; set; }
        public string NumeroResolucion { get; set; }
        public string CartaOrden { get; set; }
        public string LiquidacionIngreso { get; set; }
        public string PapeletaDeposito { get; set; }
        public string Concepto { get; set; }
        public string Referencia { get; set; }
        public int? LiquidacionId { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<ReciboIngresoDetalleFormDto> ReciboIngresoDetalle { get; set; }
        public ReciboIngresoFormDto()
        {
            ReciboIngresoDetalle = new List<ReciboIngresoDetalleFormDto>();
        }

    }
}
