using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;


namespace RecaudacionApiReciboIngreso.DataAccess
{
    public interface IReciboIngresoDetalleRepository
    {
        Task<List<ReciboIngresoDetalle>> FindAll(int id);
        Task<ReciboIngresoDetalle> FindById(int id);
        decimal SumImporte(List<ReciboIngresoDetalle> detalles);
        Task<ReciboIngresoDetalle> Add(ReciboIngresoDetalle reciboIngresoDetalle);
        Task Delete(ReciboIngresoDetalle reciboIngresoDetalle);
    }
}
