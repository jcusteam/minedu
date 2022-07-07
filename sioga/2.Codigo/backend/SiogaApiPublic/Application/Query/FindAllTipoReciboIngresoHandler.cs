using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SiogaApiPublic.Clients;
using SiogaApiPublic.Domain;
using SiogaUtils;

namespace SiogaApiPublic.Application.Query
{
    public class FindAllTipoReciboIngresoHandler
    {
        public class StatusFindAllTipoReciboIngresoResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllTipoReciboIngresoResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllTipoReciboIngresoResponse>
        {
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly AppSettings _appSettings;
            public Handler(ITipoReciboIngresoAPI tipoReciboIngresoAPI, AppSettings appSettings)
            {
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _appSettings = appSettings;
            }

            public async Task<StatusFindAllTipoReciboIngresoResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllTipoReciboIngresoResponse();
                var aes = new AES256();
                var key = _appSettings.SecretKeyAES + "SISSIOGA";

                try
                {
                    var tipoReciboIngresoResponse = await _tipoReciboIngresoAPI.FindAllAsync();
                    if (!tipoReciboIngresoResponse.Success)
                    {
                        response.Messages.Add(tipoReciboIngresoResponse.Messages[0].Message);
                        response.MessageType = tipoReciboIngresoResponse.Messages[0].Type;
                        response.Success = false;
                        return response;
                    }

                    var jsonData = JsonConvert.SerializeObject(tipoReciboIngresoResponse.Data, new JsonSerializerSettings
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
