using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;

namespace RecaudacionApiPapeletaDeposito.DataAccess
{
    public interface IPapeletaDepositoDetalleRepository
    {
        Task<List<PapeletaDepositoDetalle>> FindAll(int id);
        Task<PapeletaDepositoDetalle> FindByReciboIngreso(int reciboIngresoId);
        Task<PapeletaDepositoDetalle> FindById(int id);
        Task<PapeletaDepositoDetalle> Add(PapeletaDepositoDetalle papeletaDepositoDetalle);
        Task<int> Delete(PapeletaDepositoDetalle papeletaDepositoDetalle);
    }
}
