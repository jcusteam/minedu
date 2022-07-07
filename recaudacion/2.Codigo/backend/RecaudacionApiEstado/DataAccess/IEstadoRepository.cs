using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiEstado.Domain;
using RecaudacionUtils;

namespace RecaudacionApiEstado.DataAccess
{
    public interface IEstadoRepository
    {
        Task<List<Estado>> FindAll();
        Task<List<Estado>> FindAll(EstadoFilter filter);
        Task<int> Count(EstadoFilter filter);
        Task<Pagination<Estado>> FindPage(EstadoFilter filter);
        Task<bool> VerifyExists(int tipo, Estado estado);
        Task<List<Estado>> FindByTipoDoc(int tipoDocumentoId);
        Task<Estado> FindById(int id);
        Task<Estado> FindByTipoDocAndNumero(int tipoDocumentoId, int numero);
        Task<Estado> Add(Estado estado);
        Task Update(Estado estado);
        Task Delete(Estado estado);
    }
}
