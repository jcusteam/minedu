using System;

namespace RecaudacionApiParametro.Application.Query.Dtos
{
    public class ParametroDto
    {
        public int ParametroId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public bool Estado { get; set; }
    }
}