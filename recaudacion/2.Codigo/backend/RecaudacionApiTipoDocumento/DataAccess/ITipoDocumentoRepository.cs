using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiTipoDocumento.Domain;
using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.DataAccess
{
    public interface ITipoDocumentoRepository
    {
        Task<List<TipoDocumento>> FindAll();
        Task<List<TipoDocumento>> FindAll(TipoDocumentoFilter filter);
        Task<int> Count(TipoDocumentoFilter filter);
        Task<Pagination<TipoDocumento>> FindPage(TipoDocumentoFilter filter);
        Task<bool> VerifyExists(int tipo, TipoDocumento tipoDocumento);
        Task<TipoDocumento> FindById(int id);
        Task<TipoDocumento> Add(TipoDocumento tipoDocumento);
        Task Update(TipoDocumento tipoDocumento);
        Task Delete(TipoDocumento tipoDocumento);
        Task<int> GenerateId();
    }
}
