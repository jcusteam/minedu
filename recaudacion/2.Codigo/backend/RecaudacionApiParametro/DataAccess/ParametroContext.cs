using RecaudacionApiParametro.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiParametro.DataAccess
{
    public class ParametroContext:DbContext
    {
        public ParametroContext(DbContextOptions<ParametroContext> options) : base(options) { }
        public DbSet<Parametro> Parametros { get; set; }
    }
}