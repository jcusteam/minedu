using System;
using System.Collections.Generic;

namespace RecaudacionApiIngresoPecosa.Application.Command.Dtos
{
    public class IngresoPecosaFormDto
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
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<IngresoPecosaDetalleFormDto> IngresoPecosaDetalle { get; set; }

        public IngresoPecosaFormDto()
        {
            IngresoPecosaDetalle = new List<IngresoPecosaDetalleFormDto>();
        }
    }
}
