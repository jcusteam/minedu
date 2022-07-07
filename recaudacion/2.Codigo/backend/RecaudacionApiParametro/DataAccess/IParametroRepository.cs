using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiParametro.Domain;
using RecaudacionUtils;

namespace RecaudacionApiParametro.DataAccess
{
    public interface IParametroRepository
    {
        Task<List<Parametro>> FindAll();
        Task<List<Parametro>> FindAll(ParametroFilter filter);
        Task<int> Count(ParametroFilter filter);
        Task<Pagination<Parametro>> Findpage(ParametroFilter filter);
        Task<Parametro> FindById(int id);
        Task<Parametro> FindByEjecutoraAndTipo(int ejecutoraId, int tipoId);
        Task<bool> VerifyExists(int tipo, Parametro parametro);
        Task<Parametro> Add(Parametro Parametro);
        Task Update(Parametro Parametro);
        Task Delete(Parametro Parametro);
    }
}
