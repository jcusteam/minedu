using RecaudacionApiPapeletaDeposito.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiPapeletaDeposito.DataAccess
{
    public class PapeletaDepositoContext:DbContext
    {
        public PapeletaDepositoContext(DbContextOptions<PapeletaDepositoContext> options) : base(options) { }
        public DbSet<PapeletaDeposito> PapeletaDepositos { get; set; }
        public DbSet<PapeletaDepositoDetalle> PapeletaDepositoDetalles { get; set; }
    }
}