using RecaudacionApiEstado.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiEstado.DataAccess
{
    public class EstadoContext:DbContext
    {
        public EstadoContext(DbContextOptions<EstadoContext> options) : base(options) { }
        public DbSet<Estado> Estados { get; set; }
    }
}