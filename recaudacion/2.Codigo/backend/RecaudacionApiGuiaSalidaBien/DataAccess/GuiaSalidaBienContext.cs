using RecaudacionApiGuiaSalidaBien.Domain;
using Microsoft.EntityFrameworkCore;

namespace RecaudacionApiGuiaSalidaBien.DataAccess
{
    public class GuiaSalidaBienContext:DbContext
    {
        public GuiaSalidaBienContext(DbContextOptions<GuiaSalidaBienContext> options) : base(options) { }
        public DbSet<GuiaSalidaBien> GuiaSalidaBienes { get; set; }
        public DbSet<GuiaSalidaBienDetalle> GuiaSalidaBienDetalles { get; set; }
    }
}