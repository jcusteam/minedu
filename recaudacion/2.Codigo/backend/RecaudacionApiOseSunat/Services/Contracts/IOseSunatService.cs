
using RecaudacionUtils;
using ServiceReference1;
using System.Threading.Tasks;

namespace RecaudacionApiOseSunat.Services.Contracts
{
    public interface IOseSunatService
    {
        Task<billServiceClient> GetInstanceAsync();
        Task<StatusResponse<byte[]>> sendBill(string filename, byte[] content, string strUser, string strPass);
    }
}
