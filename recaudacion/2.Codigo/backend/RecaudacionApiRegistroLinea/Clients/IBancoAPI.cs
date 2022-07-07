using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiRegistroLinea.Clients
{
    public interface IBancoAPI
    {
        [Get("/api/bancos/{id}")]
        Task<StatusResponse<Banco>> FindByIdAsync(int id);
    }
}
