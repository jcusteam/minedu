using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.DataAccess
{
    public interface IReciboIngresoRepository
    {
        Task<List<ReciboIngreso>> FindAll();
        Task<List<ReciboIngreso>> FindAll(ReciboIngresoFilter filter);
        Task<int> Count(ReciboIngresoFilter filter);
        Task<Pagination<ReciboIngreso>> FindPage(ReciboIngresoFilter filter);
        Task<ReciboIngreso> FindById(int id);
        Task<ReciboIngreso> FindByNumeroAndEjecutoraAndCuenta(string numero, int ejecutoraId, int cuentaId);
        Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId);
        Task<List<Chart>> FindChartTipoRecibo(int tipoReciboId, int ejecutoraId, int? anio);
        Task<List<ChartTipoRecibo>> FindChart(List<TipoReciboIngreso> lista, int ejecutoraId, int? anio);
        Task<ReciboIngreso> Add(ReciboIngreso ReciboIngreso);
        Task Update(ReciboIngreso ReciboIngreso);
        Task Delete(ReciboIngreso ReciboIngreso);
    }
}
