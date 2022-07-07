using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.DataAccess
{
    public interface ILiquidacionRepository
    {
        Task<List<Liquidacion>> FindAll();
        Task<List<Liquidacion>> FindAll(LiquidacionFilter filter);
        Task<int> Count(LiquidacionFilter filter);
        Task<Pagination<Liquidacion>> FindPage(LiquidacionFilter filter);
        Task<List<LiquidacionDetalle>> FindDetalleById(int id);
        Task<List<LiquidacionDetalle>> GrupDetalleByClasificador(int id);
        Task<int> CountDetalleByFecha();
        Task<Liquidacion> FindById(int id);
        Task<string> FindNumeroCorrelativo(int unidadEjecutoraId, int tipoDocumentoId);
        Task<Liquidacion> Add(Liquidacion liquidacion);

        Task Update(Liquidacion liquidacion);
        Task Delete(Liquidacion liquidacion);
    }
}
