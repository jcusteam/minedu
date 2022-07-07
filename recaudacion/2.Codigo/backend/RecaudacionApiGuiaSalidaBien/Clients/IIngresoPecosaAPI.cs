using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiGuiaSalidaBien.Clients
{
    public interface IIngresoPecosaAPI
    {
        [Get("/api/ingreso-pecosa/detalle/{id}")]
        Task<StatusResponse<IngresoPecosaDetalle>> FindDetalleByIdAsync(int id);
    }
}
