using RecaudacionApiDepositoBanco.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiDepositoBanco.DataAccess
{
    public class DepositoBancoContext:DbContext
    {
        public DepositoBancoContext(DbContextOptions<DepositoBancoContext> options) : base(options) { }
        public DbSet<DepositoBanco> DepositoBancos { get; set; }
        public DbSet<DepositoBancoDetalle> DepositoBancoDetalles { get; set; }
    }
}