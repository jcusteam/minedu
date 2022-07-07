using System;
using System.Collections.Generic;

namespace RecaudacionApiComprobantePago.Application.Command.Dtos
{
    public class ComprobantePagoFormDto
    {
        public int ComprobantePagoId { get; set; }
        public int ClienteId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoComprobanteId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int? DepositoBancoDetalleId { get; set; }
        public int? CuentaCorrienteId { get; set; }
        public int TipoCaptacionId { get; set; }
        public int ComprobanteEmisorId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int TipoAdquisicion { get; set; }
        public string CodigoTipoOperacion { get; set; }
        public int TipoCondicionPago { get; set; }
        public string NumeroDeposito { get; set; }
        public DateTime? FechaDeposito { get; set; }
        public string ValidarDeposito { get; set; }
        public string NumeroCheque { get; set; }
        public int? EncargadoTipoDocumento { get; set; }
        public string EncargadoNombre { get; set; }
        public string EncargadoNumeroDocumento { get; set; }
        public int? FuenteId { get; set; }
        public int? FuenteTipoDocumento { get; set; }
        public string FuenteSerie { get; set; }
        public string FuenteCorrelativo { get; set; }
        public string FuenteOrigen { get; set; }
        public string FuenteValidar { get; set; }
        public string Sustento { get; set; }
        public string Observacion { get; set; }
        public string NombreArchivo { get; set; }
        public decimal TipoCambio { get; set; }
        public bool Pagado { get; set; }
        public string EstadoSunat { get; set; }
        public string CodigoTipoMoneda { get; set; }
        public decimal ImporteBruto { get; set; }
        public decimal ValorIGV { get; set; }
        public decimal IGVTotal { get; set; }
        public decimal ISCTotal { get; set; }
        public decimal OTRTotal { get; set; }
        public decimal OTRCTotal { get; set; }
        public decimal ImporteTotal { get; set; }
        public string ImporteTotalLetra { get; set; }
        public decimal TotalOpGravada { get; set; }
        public decimal TotalOpInafecta { get; set; }
        public decimal TotalOpExonerada { get; set; }
        public decimal TotalOpGratuita { get; set; }
        public decimal TotalDescuento { get; set; }
        public string OrdenCompra { get; set; }
        public string GuiaRemision { get; set; }
        public string CodigoTipoNota { get; set; }
        public string CodigoMotivoNota { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<ComprobantePagoDetalleFormDto> ComprobantePagoDetalle { get; set; }

        public ComprobantePagoFormDto()
        {
            ComprobantePagoDetalle = new List<ComprobantePagoDetalleFormDto>();
        }
    }
}
