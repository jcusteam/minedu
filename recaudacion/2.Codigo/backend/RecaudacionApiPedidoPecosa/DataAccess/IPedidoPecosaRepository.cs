using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiPedidoPecosa.Domain;

namespace RecaudacionApiPedidoPecosa.DataAccess
{
    public interface IPedidoPecosaRepository
    {
        Task<List<PedidoPecosa>> FindAll(string ejecutora, int anioEje, int numeroPecosa);
        Task<PedidoPecosa> FindByEjecutora(string ejecutora, int anioEje, int numeroPecosa);
    }
}
