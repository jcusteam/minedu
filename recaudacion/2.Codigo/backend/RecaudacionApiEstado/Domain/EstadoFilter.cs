using RecaudacionUtils;

namespace RecaudacionApiEstado.Domain
{
    public class EstadoFilter : BaseFilter
    {
        public int? TipoDocumentoId { get; set; }
        public string Nombre { get; set; }
    }
}
