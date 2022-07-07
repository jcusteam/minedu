using System;
using System.Collections.Generic;

namespace RecaudacionApiComprobanteRetencion.Application.Command.Dtos
{
    public class ComprobanteRetencionEstadoFormDto
    {
        public int ComprobanteRetencionId { get; set; }
        public int Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
