using System;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Dtos
{
    public class ComprobanteEmisorEstadoFormDto
    {
        public int ComprobanteEmisorId { get; set; }
        public bool Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
