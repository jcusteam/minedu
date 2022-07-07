using System;
using System.Collections.Generic;

namespace RecaudacionApiGuiaSalidaBien.Application.Command.Dtos
{
    public class GuiaSalidaBienEstadoFormDto
    {
        public int GuiaSalidaBienId { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
