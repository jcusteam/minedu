using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaApiPublic.Clients;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Application.Query
{
    public class FindAllCuentaCorrienteHandler
    {
        public class StatusFindAllCuentaCorrienteResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllCuentaCorrienteResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllCuentaCorrienteResponse>
        {
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly AppSettings _appSettings;
            public Handler(ICuentaCorrienteAPI cuentaCorrienteAPI, AppSettings appSettings)
            {
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _appSettings = appSettings;
            }

            public async Task<StatusFindAllCuentaCorrienteResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllCuentaCorrienteResponse();
                var aes = new AES256();
                var key = _appSettings.SecretKeyAES + "SISSIOGA";

                try
                {
                    var cuentaCorrienteResponse = await _cuentaCorrienteAPI.FindAllAsync();

                    if (!cuentaCorrienteResponse.Success)
                    {
                        response.Messages.Add(cuentaCorrienteResponse.Messages[0].Message);
                        response.MessageType = cuentaCorrienteResponse.Messages[0].Type;
                        response.Success = false;
                        return response;
                    }

                    var jsonData = JsonConvert.SerializeObject(cuentaCorrienteResponse.Data, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                    response.Data = aes.Encrypt(jsonData, key);

                }
                catch (System.Exception)
                {
                    response.Messages.Add(Message.ERROR_SERVICE);
                    response.MessageType = Definition.MESSAGE_TYPE_ERROR;
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
