using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.Application.Query.Dtos
{
    public class TipoDocumentoFilterDto : BaseFilter
    {
        public string Nombre { get; set; }
        public bool? Estado { get; set; }

    }
}
