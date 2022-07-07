using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionUtils;
using Refit;

namespace RecaudacionApiIngresoPecosa.Clients
{
    public interface ICatalogoBienAPI
    {
        [Get("/api/catalogo-bienes/paginar")]
        Task<StatusResponse<Pagination<CatalogoBien>>> FindPageAsync([FromQuery] CatalogoBienFilter filter);
        [Get("/api/catalogo-bienes")]
        Task<StatusResponse<List<CatalogoBien>>> FindAllAsync();
        [Get("/api/catalogo-bienes/{id}")]
        Task<StatusResponse<CatalogoBien>> FindByIdAsync(int id);
        [Get("/api/catalogo-bienes/consulta")]
        Task<StatusResponse<CatalogoBien>> FindByCodigoAsync([FromQuery] string codigo);
        [Post("/api/catalogo-bienes")]
        Task<StatusResponse<CatalogoBien>> AddAsync(CatalogoBien catalogoBien);

    }
}
