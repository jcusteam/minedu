using RecaudacionApiTipoDocumento.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiTipoDocumento.DataAccess
{
    public class TipoDocumentoContext:DbContext
    {
        public TipoDocumentoContext(DbContextOptions<TipoDocumentoContext> options) : base(options) { }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
    }
}