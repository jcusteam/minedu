using RecaudacionApiComprobanteEmisor.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiComprobanteEmisor.DataAccess
{
    public class ComprobanteEmisorContext : DbContext
    {
        public ComprobanteEmisorContext(DbContextOptions<ComprobanteEmisorContext> options) : base(options) { }
        public DbSet<ComprobanteEmisor> ComprobanteEmisors { get; set; }
    }
}
