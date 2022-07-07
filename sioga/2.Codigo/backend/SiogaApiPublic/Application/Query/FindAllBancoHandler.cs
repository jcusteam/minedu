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
    public class FindAllBancoHandler
    {
        public class StatusFindAllBancoResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllBancoResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllBancoResponse>
        {
            private readonly IBancoAPI _bancoAPI;
            private readonly AppSettings _appSettings;
            public Handler(IBancoAPI bancoAPI, AppSettings appSettings)
            {
                _bancoAPI = bancoAPI;
                _appSettings = appSettings;
            }

            public async Task<StatusFindAllBancoResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllBancoResponse();
                var aes = new AES256();
                var key = _appSettings.SecretKeyAES + "SISSIOGA";
                try
                {
                    var bancoResponse = await _bancoAPI.FindAllAsync();

                    if (!bancoResponse.Success)
                    {
                        response.Messages.Add(bancoResponse.Messages[0].Message);
                        response.MessageType = bancoResponse.Messages[0].Type;
                        response.Success = false;
                        return response;
                    }

                    var jsonData = JsonConvert.SerializeObject(bancoResponse.Data, new JsonSerializerSettings
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
