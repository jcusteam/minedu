using RecaudacionApiReciboIngreso.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiReciboIngreso.DataAccess
{
    public class ReciboIngresoContext : DbContext
    {
        public ReciboIngresoContext(DbContextOptions<ReciboIngresoContext> options) : base(options) { }
        public DbSet<ReciboIngreso> ReciboIngresos { get; set; }
        public DbSet<ReciboIngresoDetalle> ReciboIngresoDetalles { get; set; }
    }
}