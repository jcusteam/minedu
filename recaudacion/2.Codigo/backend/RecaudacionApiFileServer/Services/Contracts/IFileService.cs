using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using RecaudacionApiFileServer.Domain;
using RecaudacionUtils;

namespace RecaudacionApiFileServer.Services.Contracts
{
    public interface IFileService
    {
        Task<StatusResponse<string>> SaveFile(IFormFile file, string subDirectory);
        Task<StatusResponse<FileContent>> FindByNameFile(string subDirectory, string fileName);
    }
}
