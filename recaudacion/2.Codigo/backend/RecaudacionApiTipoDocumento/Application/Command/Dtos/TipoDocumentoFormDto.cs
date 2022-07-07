using System;

namespace RecaudacionApiTipoDocumento.Application.Command.Dtos
{
    public class TipoDocumentoFormDto
    {
        public int TipoDocumentoId { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public bool Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}