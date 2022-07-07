using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.DataAccess
{
    public interface IIngresoPecosaRepository
    {
        Task<List<IngresoPecosa>> FindAll();
        Task<List<IngresoPecosa>> FindAll(IngresoPecosaFilter filter);
        Task<int> Count(IngresoPecosaFilter filter);
        Task<IngresoPecosa> FindById(int id);
        Task<Pagination<IngresoPecosa>> FindPage(IngresoPecosaFilter filter);
        Task<bool> VerifyExists(int tipo, IngresoPecosa ingresoPecosa);
        Task<IngresoPecosa> Add(IngresoPecosa ingresoPecosa);
        Task Update(IngresoPecosa ingresoPecosa);
        Task Delete(IngresoPecosa ingresoPecosa);
    }
}
