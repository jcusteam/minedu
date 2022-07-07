using System;
using System.Collections.Generic;

namespace RecaudacionApiComprobantePago.Application.Command.Dtos
{
    public class ComprobantePagoEstadoFormDto
    {
        public int ComprobantePagoId { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
