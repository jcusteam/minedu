using System;
using System.Collections.Generic;
using RecaudacionApiLiquidacion.Domain;

namespace RecaudacionApiLiquidacion.Application.Query.Dtos
{
    public class LiquidacionDto
    {
        public int LiquidacionId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int FuenteFinanciamientoId { get; set; }
        public FuenteFinanciamiento FuenteFinanciamiento { get; set; }
        public int TipoDocumentoId { get; set; }
        public int ClienteId { get; set; }
        public int CuentaCorrienteId { get; set; }
        public int? ReciboIngresoId { get; set; }
        public string Numero { get; set; }
        public string Procedencia { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal Total { get; set; }
        public string Factura { get; set; }
        public string BoletaVenta { get; set; }
        public int Estado { get; set; }
        public string EstadoNombre { get; set; }
        public List<LiquidacionDetalleDto> LiquidacionDetalle { get; set; }
        public LiquidacionDto()
        {
            LiquidacionDetalle = new List<LiquidacionDetalleDto>();
        }
    }
}
