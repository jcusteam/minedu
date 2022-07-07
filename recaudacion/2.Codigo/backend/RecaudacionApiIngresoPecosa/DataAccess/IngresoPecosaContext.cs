using RecaudacionApiIngresoPecosa.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiIngresoPecosa.DataAccess
{
    public class IngresoPecosaContext:DbContext
    {
        public IngresoPecosaContext(DbContextOptions<IngresoPecosaContext> options) : base(options) { }
        public DbSet<IngresoPecosa> IngresoPecosas { get; set; }
        public DbSet<IngresoPecosaDetalle> IngresoPecosaDetalles { get; set; }
    }
}