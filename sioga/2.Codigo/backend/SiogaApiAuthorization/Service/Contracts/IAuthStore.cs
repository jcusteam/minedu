using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace SiogaApiAuthorization.Service.Contracts
{
    public interface IAuthStore
    {
        Task<string> GetStore(string key);

        bool AddStore(string key, string value);
    }
}
