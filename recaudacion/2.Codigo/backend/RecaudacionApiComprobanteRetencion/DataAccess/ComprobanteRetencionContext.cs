using RecaudacionApiComprobanteRetencion.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiComprobanteRetencion.DataAccess
{
    public class ComprobanteRetencionContext:DbContext
    {
        public ComprobanteRetencionContext(DbContextOptions<ComprobanteRetencionContext> options) : base(options) { }
        public DbSet<ComprobanteRetencion> ComprobanteRetenciones { get; set; }
        public DbSet<ComprobanteRetencionDetalle> ComprobanteRetencionDetalles { get; set; }
    }
}