using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteRetencion.DataAccess
{
    public interface IComprobanteRetencionRepository
    {
        Task<List<ComprobanteRetencion>> FindAll();
        Task<List<ComprobanteRetencion>> FindAll(ComprobanteRetencionFilter filter);
        Task<int> Count(ComprobanteRetencionFilter filter);
        Task<Pagination<ComprobanteRetencion>> FindPage(ComprobanteRetencionFilter filter);
        Task<ComprobanteRetencion> FindById(int id);
        Task<ComprobanteRetencionParametro> FindParametro(int ejecutoraId, int tipoDocumentoId);
        Task<ComprobanteRetencion> Add(ComprobanteRetencion ComprobanteRetencion);
        Task Update(ComprobanteRetencion ComprobanteRetencion);
        Task Delete(ComprobanteRetencion ComprobanteRetencion);
    }
}
