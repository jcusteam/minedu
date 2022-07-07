using System;

namespace RecaudacionApiLiquidacion.Domain
{
    public class TipoDocumento
    {
        public int TipoDocumentoId { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public bool Estado { get; set; }
    }
}
