using System;

namespace RecaudacionApiParametro.Application.Command.Dtos
{
    public class ParametroFormDto
    {
        public int ParametroId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public bool Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
