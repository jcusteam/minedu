using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.DataAccess
{
    public interface IDepositoBancoRepository
    {
        Task<List<DepositoBanco>> FindAll();
        Task<List<DepositoBanco>> FindAll(DepositoBancoFilter filter);
        Task<int> Count(DepositoBancoFilter filter);
        Task<Pagination<DepositoBanco>> FindPage(DepositoBancoFilter filter);
        Task<DepositoBanco> FindById(int id);
        Task<string> FindCorrelativo(int ejecutoraId, int tipoDocumentoId);
        Task<bool> VerifyExistsNombreArchivo(string nombreArchivo);
        Task<DepositoBanco> Add(DepositoBanco DepositoBanco);
        Task Update(DepositoBanco DepositoBanco);
        Task Delete(DepositoBanco DepositoBanco);
    }
}
