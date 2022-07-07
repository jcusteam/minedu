using System;

namespace SiogaApiPublic.Domain
{
    public class TipoDocumentoIdentidad
    {
        public int TipoDocumentoIdentidadId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}