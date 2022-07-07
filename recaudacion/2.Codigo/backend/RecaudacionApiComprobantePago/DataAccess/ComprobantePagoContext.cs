using RecaudacionApiComprobantePago.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiComprobantePago.DataAccess
{
    public class ComprobantePagoContext:DbContext
    {
        public ComprobantePagoContext(DbContextOptions<ComprobantePagoContext> options) : base(options) { }
        public DbSet<ComprobantePago> ComprobantePagos { get; set; }
         public DbSet<ComprobantePagoDetalle> ComprobantePagoDetalles { get; set; }
    }
}