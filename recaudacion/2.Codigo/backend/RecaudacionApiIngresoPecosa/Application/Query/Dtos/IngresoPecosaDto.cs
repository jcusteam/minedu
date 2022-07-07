using System;
using System.Collections.Generic;

namespace RecaudacionApiIngresoPecosa.Application.Query.Dtos
{
    public class IngresoPecosaDto
    {
        public int IngresoPecosaId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int AnioPecosa { get; set; }
        public string TipoBien { get; set; }
        public int NumeroPecosa { get; set; }
        public DateTime FechaPecosa { get; set; }
        public string NombreAlmacen { get; set; }
        public string MotivoPedido { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int Estado { get; set; }
        public string EstadoNombre { get; set; }
        public List<IngresoPecosaDetalleDto> IngresoPecosaDetalle { get; set; }

        public IngresoPecosaDto()
        {
            IngresoPecosaDetalle = new List<IngresoPecosaDetalleDto>();
        }
    }
}
