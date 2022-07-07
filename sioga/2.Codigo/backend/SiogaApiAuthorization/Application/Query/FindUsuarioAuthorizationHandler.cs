using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;
using FluentValidation;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindUsuarioAuthorizationHandler
    {
        public class StatusUsuarioResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusUsuarioResponse>
        {
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

        public class Handler : IRequestHandler<Query, StatusUsuarioResponse>
        {
            private readonly IAutorizationService _authorizationService;

            public Handler(IAutorizationService authorizationService
            )
            {
                _authorizationService = authorizationService;
            }

            public async Task<StatusUsuarioResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusUsuarioResponse();
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

                    var usuarioResponse = await _authorizationService.GetUsuario(request.HeaderAuth);
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
