using System;

namespace RecaudacionApiLiquidacion.Application.Command.Dtos
{
    public class LiquidacionFormDto
    {
        public int LiquidacionId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int FuenteFinanciamientoId { get; set; }
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
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
