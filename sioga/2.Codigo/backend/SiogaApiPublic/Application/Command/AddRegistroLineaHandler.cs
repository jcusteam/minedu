using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SiogaApiPublic.Domain;
using SiogaApiPublic.Clients;
using SiogaUtils;
using Newtonsoft.Json;

namespace SiogaApiPublic.Application.Command
{
    public class AddRegistroLineaHandler
    {
        public class StatusAddRegistroLineaResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddRegistroLineaResponse>
        {
            public DataModel FormDto { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.FormDto.data)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Data es requerido");
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddRegistroLineaResponse>
        {
            private readonly IRegistroLineaAPI _registroLineaAPI;
            private readonly AppSettings _appSettings;
            public Handler(IRegistroLineaAPI registroLineaAPI, AppSettings appSettings)
            {
                _registroLineaAPI = registroLineaAPI;
                _appSettings = appSettings;
            }

            public async Task<StatusAddRegistroLineaResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddRegistroLineaResponse();
                var aes = new AES256();
                var key = _appSettings.SecretKeyAES + "SISSIOGA";
                var registroLinea = new RegistroLinea();
                try
                {
                    try
                    {
                        var decriptData = aes.Decrypt(request.FormDto.data, key);
                        registroLinea = JsonConvert.DeserializeObject<RegistroLinea>(decriptData);
                    }
                    catch (System.Exception)
                    {
                        response.Messages.Add("Ocurrio un error al realizar el desencriptado de Registro en Linea");
                        response.MessageType = Definition.MESSAGE_TYPE_ERROR;
                        response.Success = false;
                        return response;
                    }

                    var registroLinaReponse = await _registroLineaAPI.AddAsync(registroLinea);
                    response.Messages.Add(registroLinaReponse.Messages[0].Message);
                    response.MessageType = registroLinaReponse.Messages[0].Type;
                    response.Success = registroLinaReponse.Success;

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
