using System;
using System.Collections.Generic;

namespace RecaudacionApiGuiaSalidaBien.Application.Query.Dtos
{
    public class GuiaSalidaBienDto
    {
        public int GuiaSalidaBienId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Justificacion { get; set; }
        public int Estado { get; set; }
        public string EstadoNombre { get; set; }
        public List<GuiaSalidaBienDetalleDto> GuiaSalidaBienDetalle { get; set; }
        public GuiaSalidaBienDto()
        {
            GuiaSalidaBienDetalle = new List<GuiaSalidaBienDetalleDto>();
        }
    }
}