using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Domain;

namespace RecaudacionApiIngresoPecosa.DataAccess
{
    public interface IIngresoPecosaDetalleRepository
    {
        Task<IEnumerable<IngresoPecosaDetalle>> FindAll(int id);
        Task<List<IngresoPecosaDetalle>> FindByCatalogoBienSaldo(int catalogoBienId);
        Task<int> CountByCatalogoBienSaldo(int catalogoBienId);
        Task<IngresoPecosaDetalle> FindById(int id);
        Task<IngresoPecosaDetalle> Add(IngresoPecosaDetalle ingresoPecosaDetalle);
        Task Update(IngresoPecosaDetalle ingresoPecosaDetalle);
    }
}