using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;

namespace RecaudacionApiRegistroLinea.DataAccess
{
    public interface IRegistroLineaDetalleRepository
    {
        Task<List<RegistroLineaDetalle>> FindAll(int id);
        Task<RegistroLineaDetalle> FindById(int id);
        decimal SumImporte(List<RegistroLineaDetalle> detalles);
        Task<RegistroLineaDetalle> Add(RegistroLineaDetalle registroLineaDetalle);
        Task Delete(RegistroLineaDetalle registroLineaDetalle);
    }
}
