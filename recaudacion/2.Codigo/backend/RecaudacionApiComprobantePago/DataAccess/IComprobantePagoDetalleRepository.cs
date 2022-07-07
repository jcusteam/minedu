using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;

namespace RecaudacionApiComprobantePago.DataAccess
{
    public interface IComprobantePagoDetalleRepository
    {
        Task<IEnumerable<ComprobantePagoDetalle>> FindAll(int id);
        Task<ComprobantePagoDetalle> FindById(int id);
        Task<ComprobantePagoDetalle> Add(ComprobantePagoDetalle comprobantePagoDetalle);
    }
}
