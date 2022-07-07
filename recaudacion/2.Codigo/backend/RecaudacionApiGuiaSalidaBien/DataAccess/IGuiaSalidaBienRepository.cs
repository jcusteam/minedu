using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionUtils;

namespace RecaudacionApiGuiaSalidaBien.DataAccess
{
    public interface IGuiaSalidaBienRepository
    {
        Task<List<GuiaSalidaBien>> FindAll();
        Task<List<GuiaSalidaBien>> FindAll(GuiaSalidaBienFilter filter);
        Task<int> Count(GuiaSalidaBienFilter filter);
        Task<Pagination<GuiaSalidaBien>> FindPage(GuiaSalidaBienFilter filter);
        Task<GuiaSalidaBien> FindById(int id);
        Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId);
        Task<GuiaSalidaBien> Add(GuiaSalidaBien GuiaSalidaBien);
        Task Update(GuiaSalidaBien GuiaSalidaBien);
        Task Delete(GuiaSalidaBien GuiaSalidaBien);
    }
}
