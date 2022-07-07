using System;

namespace RecaudacionApiTipoDocumento.Application.Query.Dtos
{
    public class TipoDocumentoDto
    {
        public int TipoDocumentoId { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public bool Estado { get; set; }
    }
}
