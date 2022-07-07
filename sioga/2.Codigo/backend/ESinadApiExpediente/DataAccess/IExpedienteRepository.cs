using System.Threading.Tasks;
using ESinadApiExpediente.Domain;

namespace ESinadApiExpediente.DataAccess
{
    public interface IExpedienteRepository
    {
        Task<Expediente> FindByNumeroExpediente(string numeroExpediente);
    }
}