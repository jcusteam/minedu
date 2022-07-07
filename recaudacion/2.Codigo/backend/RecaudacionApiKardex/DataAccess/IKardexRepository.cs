using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiKardex.Domain;

namespace RecaudacionApiKardex.DataAccess
{
    public interface IKardexRepository
    {
        Task<Kardex> FindById(int id, int catalogoBienId);
        Task<List<Kardex>> FindAll(int catalogoBienId);
    }
}
