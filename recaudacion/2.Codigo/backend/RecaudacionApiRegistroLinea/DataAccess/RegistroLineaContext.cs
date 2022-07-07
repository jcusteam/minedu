using RecaudacionApiRegistroLinea.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiRegistroLinea.DataAccess
{
    public class RegistroLineaContext:DbContext
    {
        public RegistroLineaContext(DbContextOptions<RegistroLineaContext> options) : base(options) { }
        public DbSet<RegistroLinea> RegistroLineas { get; set; }
        public DbSet<RegistroLineaDetalle> RegistroLineaDetalles { get; set; }
    }
}