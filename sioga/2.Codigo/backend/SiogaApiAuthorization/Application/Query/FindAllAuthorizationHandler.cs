using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaApiAuthorization.Application.Query.Dtos;
using SiogaUtils;
using FluentValidation;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindAllAuthorizationHandler
    {
        public class StatusFindAllResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
            public AuthDto AuthDto { get; set; }
            public string HeaderAuth { get; set; }
        }

        public class CommandValidator : AbstractValidator<Query>
        {
            public CommandValidator()
            {
                RuleFor(x => x.HeaderAuth)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Authorization Bearer es requerido");
            }
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IAutorizationService _authorizationService;
            private readonly IAuthStore _authStore;

            public Handler(IAutorizationService authorizationService, IAuthStore authStore)
            {
                _authorizationService = authorizationService;
                _authStore = authStore;
            }

            public async Task<StatusFindAllResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindAllResponse();
                try
                {

                    CommandValidator validations = new CommandValidator();
                    var result = validations.Validate(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var usuarioResponse = await _authorizationService.GetAllUsuario(request.AuthDto.CodigoModulo, request.HeaderAuth);

                    if (usuarioResponse.Success)
                    {
                        var guid = Guid.NewGuid().ToString();
                        usuarioResponse.Data.IdUsuario = guid;
                        var jsonData = JsonConvert.SerializeObject(usuarioResponse.Data, new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                        var usuarioStore = usuarioResponse.Data.NumeroDocumento;
                        _authStore.AddStore(guid, jsonData);
                    }
                    response.Data = usuarioResponse.Data;
                    response.Messages = usuarioResponse.Messages;
                    response.Success = usuarioResponse.Success;

                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_AUTH));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
