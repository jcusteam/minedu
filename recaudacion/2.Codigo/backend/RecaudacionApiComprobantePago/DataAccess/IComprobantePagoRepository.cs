using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.DataAccess
{
    public interface IComprobantePagoRepository
    {
        Task<List<ComprobantePago>> FindAll();
        Task<List<ComprobantePago>> FindAll(ComprobantePagoFilter filter);
        Task<Pagination<ComprobantePago>> FindPage(ComprobantePagoFilter filter);
        Task<int> Count(ComprobantePagoFilter filter);
        Task<ComprobantePago> FindById(int id);
        Task<ComprobantePago> FindByFuente(ComprobantePago ComprobantePago);
        Task<int> CountByFuente(int fuenteId);
        Task<List<Chart>> FindChartByTipo(int tipoId, int ejecutoraId, int anio);
        Task<ComprobantePagoParametro> FindParametro(int ejecutoraId, int tipo);
        Task<bool> VerifyExists(int tipo, ComprobantePago ComprobantePago);
        Task<ComprobantePago> Add(ComprobantePago ComprobantePago);
        Task Update(ComprobantePago ComprobantePago);
        Task Delete(ComprobantePago ComprobantePago);
    }
}
