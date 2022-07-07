using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;
using FluentValidation;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindSesionAuthorizationHandler
    {
        public class StatusSesionResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusSesionResponse>
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

        public class Handler : IRequestHandler<Query, StatusSesionResponse>
        {
            private readonly IAutorizationService _authorizationService;

            public Handler(IAutorizationService authorizationService)
            {
                _authorizationService = authorizationService;
            }

            public async Task<StatusSesionResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusSesionResponse();
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

                    var sesionResponse = await _authorizationService.GetSesion(request.HeaderAuth);
                    response.Data = sesionResponse.Data;
                    response.Messages = sesionResponse.Messages;
                    response.Success = sesionResponse.Success;
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
