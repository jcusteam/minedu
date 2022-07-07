using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;

namespace RecaudacionApiGuiaSalidaBien.DataAccess
{
    public interface IGuiaSalidaBienDetalleRepository
    {
        Task<List<GuiaSalidaBienDetalle>> FindAll(int id);
        Task<GuiaSalidaBienDetalle> FindById(int id);
        Task<GuiaSalidaBienDetalle> Add(GuiaSalidaBienDetalle guiaSalidaBienDetalle);
    }
}
