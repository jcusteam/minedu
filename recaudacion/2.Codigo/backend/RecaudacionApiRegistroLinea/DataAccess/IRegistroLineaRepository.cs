using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.DataAccess
{
    public interface IRegistroLineaRepository
    {
        Task<List<RegistroLinea>> FindAll();
        Task<List<RegistroLinea>> FindAll(RegistroLineaFilter filter);
        Task<int> Count(RegistroLineaFilter filter);
        Task<Pagination<RegistroLinea>> FindPage(RegistroLineaFilter filter);
        Task<RegistroLinea> FindById(int id);
        Task<bool> VerifyExistsESinad(string expedienteSinad);
        Task<bool> VerifyExists(int tipo, RegistroLinea registroLinea);
        Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId);
        Task<RegistroLinea> Add(RegistroLinea registroLinea);
        Task Update(RegistroLinea registroLinea);
        Task Delete(RegistroLinea registroLinea);
    }
}
