using System;

namespace RecaudacionApiTipoDocumento.Application.Command.Dtos
{
    public class TipoDocumentoEstadoFormDto
    {
        public int TipoDocumentoId { get; set; }
        public bool Estado { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
