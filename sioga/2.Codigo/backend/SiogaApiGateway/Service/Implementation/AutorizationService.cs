using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiogaApiGateway.Helpers;
using SiogaApiGateway.RestClients;
using SiogaApiGateway.Service.Contracts;
using SiogaUtils;

namespace SiogaApiGateway.Service.Implementation
{
    public class AutorizationService : IAutorizationService
    {
        private readonly IAuthorizationAPI _authorizationAPI;
        public AutorizationService(IAuthorizationAPI authorizationAPI)
        {
            _authorizationAPI = authorizationAPI;
        }

        public async Task<StatusApiResponse<DataUser>> GetUsuario(DataAuth dataAuth, string headerAuth)
        {
            var response = new StatusApiResponse<DataUser>();
            var usuarioResponse = await _authorizationAPI.GetUsuario(dataAuth, headerAuth);
            var rolResponse = await _authorizationAPI.GetRoles(dataAuth, headerAuth);
            var moduloResponse = await _authorizationAPI.GetModulos(dataAuth, headerAuth);

            if (!usuarioResponse.Success)
            {
                response.Success = false;
                response.Messages = usuarioResponse.Messages;
                return response;
            }

            if (!rolResponse.Success)
            {
                response.Success = false;
                response.Messages = rolResponse.Messages;
                return response;
            }

            if (!moduloResponse.Success)
            {
                response.Success = false;
                response.Messages = moduloResponse.Messages;
                return response;
            }

            var dataUser = new DataUser();
            dataUser.NumeroDocumento = usuarioResponse.Data.NumeroDocumento;

            if (moduloResponse.Data.Count > 0)
            {
                dataUser.Modulos = moduloResponse.Data.Select(x => x.Codigo).ToList();
            }

            if (rolResponse.Data.Count > 0)
            {
                dataUser.Roles = rolResponse.Data.Select(x => x.Codigo).ToList();
            }

            response.Data = dataUser;

            return response;
        }
    }
}
