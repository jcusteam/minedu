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
    public class FindAllTipoDocIdentidadHandler
    {
        public class StatusFindAllTipoDocIdentidadResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllTipoDocIdentidadResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllTipoDocIdentidadResponse>
        {
            private readonly ITipoDocumentoIdentidadAPI _tipoDocIdentidadAPI;
            private readonly AppSettings _appSettings;
            public Handler(ITipoDocumentoIdentidadAPI tipoDocIdentidadAPI, AppSettings appSettings)
            {
                _tipoDocIdentidadAPI = tipoDocIdentidadAPI;
                _appSettings = appSettings;
            }

            public async Task<StatusFindAllTipoDocIdentidadResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllTipoDocIdentidadResponse();
                var aes = new AES256();
                var key = _appSettings.SecretKeyAES + "SISSIOGA";

                try
                {
                    var tipoDocIdentidadResponse = await _tipoDocIdentidadAPI.FindAllAsync();
                    if (!tipoDocIdentidadResponse.Success)
                    {
                        response.Messages.Add(tipoDocIdentidadResponse.Messages[0].Message);
                        response.MessageType = tipoDocIdentidadResponse.Messages[0].Type;
                        response.Success = false;
                        return response;
                    }

                    var jsonData = JsonConvert.SerializeObject(tipoDocIdentidadResponse.Data, new JsonSerializerSettings
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
