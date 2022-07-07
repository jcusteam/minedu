using RecaudacionApiLiquidacion.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiLiquidacion.DataAccess
{
    public class LiquidacionContext:DbContext
    {
        public LiquidacionContext(DbContextOptions<LiquidacionContext> options) : base(options) { }
        public DbSet<Liquidacion> Liquidacions { get; set; }
    }
}