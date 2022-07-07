using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.Domain;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteEmisor.DataAccess
{
    public interface IComprobanteEmisorRepository
    {
        Task<List<ComprobanteEmisor>> FindAll();
        Task<List<ComprobanteEmisor>> FindAll(ComprobanteEmisorFilter filter);
        Task<int> Count(ComprobanteEmisorFilter filter);
        Task<Pagination<ComprobanteEmisor>> FindPage(ComprobanteEmisorFilter filter);
        Task<ComprobanteEmisor> FindById(int id);
        Task<ComprobanteEmisor> FindByUnidadEjecutoraId(int unidadEjecutoraId);
        Task<bool> VerifyExists(int tipo, ComprobanteEmisor comprobanteEmisor);
        Task<ComprobanteEmisor> Add(ComprobanteEmisor comprobanteEmisor);
        Task Update(ComprobanteEmisor comprobanteEmisor);
        Task Delete(ComprobanteEmisor comprobanteEmisor);
        Task<int> GenerateId();
    }
}
