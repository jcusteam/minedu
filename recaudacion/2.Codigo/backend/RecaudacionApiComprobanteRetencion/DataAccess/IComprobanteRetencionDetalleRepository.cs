using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;

namespace RecaudacionApiComprobanteRetencion.DataAccess
{
    public interface IComprobanteRetencionDetalleRepository
    {
        Task<IEnumerable<ComprobanteRetencionDetalle>> FindAll(int id);
        Task<ComprobanteRetencionDetalle> FindById(int id);
        Task<ComprobanteRetencionDetalle> Add(ComprobanteRetencionDetalle comprobanteRetencionDetalle);
        
    }
}