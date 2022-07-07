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
    public class FindAllClasificadorIngresoHandler
    {
        public class StatusFindAllClasificadorIngresoResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllClasificadorIngresoResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllClasificadorIngresoResponse>
        {
            private readonly IClasificadorIngresoAPI _clasificadorIngresoAPI;
            private readonly AppSettings _appSettings;
            public Handler(IClasificadorIngresoAPI clasificadorIngresoAPI, AppSettings appSettings)
            {
                _clasificadorIngresoAPI = clasificadorIngresoAPI;
                _appSettings = appSettings;
            }

            public async Task<StatusFindAllClasificadorIngresoResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllClasificadorIngresoResponse();
                var aes = new AES256();
                var key = _appSettings.SecretKeyAES + "SISSIOGA";

                try
                {
                    var clasificadorIngresoResponse = await _clasificadorIngresoAPI.FindAllAsync();
                    if (!clasificadorIngresoResponse.Success)
                    {
                        response.Messages.Add(clasificadorIngresoResponse.Messages[0].Message);
                        response.MessageType = clasificadorIngresoResponse.Messages[0].Type;
                        response.Success = false;
                        return response;
                    }

                    var jsonData = JsonConvert.SerializeObject(clasificadorIngresoResponse.Data, new JsonSerializerSettings
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
