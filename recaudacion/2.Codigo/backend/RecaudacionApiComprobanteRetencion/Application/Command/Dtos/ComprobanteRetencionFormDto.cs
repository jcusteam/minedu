using System;
using System.Collections.Generic;

namespace RecaudacionApiComprobanteRetencion.Application.Command.Dtos
{
    public class ComprobanteRetencionFormDto
    {
        public int ComprobanteRetencionId { get; set; }
        public int ClienteId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int TipoComprobanteId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime Periodo { get; set; }
        public string RegimenRetencion { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPago { get; set; }
        public decimal Porcentaje { get; set; }
        public string NombreArchivo { get; set; }
        public string Observacion { get; set; }
        public string EstadoSunat { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<ComprobanteRetencionDetalleFormDto> ComprobanteRetencionDetalle { get; set; }

        public ComprobanteRetencionFormDto()
        {
            ComprobanteRetencionDetalle = new List<ComprobanteRetencionDetalleFormDto>();
        }

    }
}
