using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.Domain
{
    public class TipoDocumentoFilter : BaseFilter
    {
        public string Nombre { get; set; }
        public bool? Estado { get; set; }
    }
}
