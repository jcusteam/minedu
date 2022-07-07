using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiDepositoBanco.Clients
{
    public interface IPideAPI
    {
        [Get("/api/pide/migraciones/consultas/{numero}")]
        Task<StatusResponse<Migracion>> FindMigracionByNumeroAsync(string numero);

        [Get("/api/pide/reniec/consultas/{dni}")]
        Task<StatusResponse<Reniec>> FindReniecByDniAsync(string dni);

        [Get("/api/pide/sunat/consultas/{numeroRuc}")]
        Task<StatusResponse<Sunat>> FindSunatByRucAsync(string numeroRuc);

    }
}
