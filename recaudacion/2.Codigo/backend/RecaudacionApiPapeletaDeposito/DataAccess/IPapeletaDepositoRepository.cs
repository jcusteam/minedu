using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.DataAccess
{
    public interface IPapeletaDepositoRepository
    {
        Task<List<PapeletaDeposito>> FindAll();
        Task<List<PapeletaDeposito>> FindAll(PapeletaDepositoFilter filter);
        Task<int> Count(PapeletaDepositoFilter filter);
        Task<Pagination<PapeletaDeposito>> FindPage(PapeletaDepositoFilter filter);
        Task<PapeletaDeposito> FindById(int id);
        Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId);
        Task<PapeletaDeposito> Add(PapeletaDeposito PapeletaDeposito);
        Task Update(PapeletaDeposito PapeletaDeposito);
        Task Delete(PapeletaDeposito PapeletaDeposito);
    }
}
