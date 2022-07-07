using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiComprobanteEmisor.Clients
{
    public interface IPideAPI
    {

        [Get("/api/pide/sunat/consultas/{numeroRuc}")]
        Task<StatusResponse<Sunat>> FindSunatByRucAsync(string numeroRuc);

    }
}
