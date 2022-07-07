using System;
using System.Collections.Generic;

namespace RecaudacionApiGuiaSalidaBien.Application.Command.Dtos
{
    public class GuiaSalidaBienFormDto
    {
        public int GuiaSalidaBienId { get; set; }
        public int UnidadEjecutoraId { get; set; }
         public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Justificacion { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<GuiaSalidaBienDetalleFormDto> GuiaSalidaBienDetalle { get; set; }

        public GuiaSalidaBienFormDto(){
            GuiaSalidaBienDetalle = new List<GuiaSalidaBienDetalleFormDto>();
        }
    }
}