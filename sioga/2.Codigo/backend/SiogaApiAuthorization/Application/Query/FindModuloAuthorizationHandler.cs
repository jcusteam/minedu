using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;
using FluentValidation;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindModuloAuthorizationHandler
    {
        public class StatusModuloResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusModuloResponse>
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

        public class Handler : IRequestHandler<Query, StatusModuloResponse>
        {
            private readonly IAutorizationService _autorizationService;

            public Handler(IAutorizationService autorizationService
            )
            {
                _autorizationService = autorizationService;
            }

            public async Task<StatusModuloResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusModuloResponse();

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
                    var moduloResponse = await _autorizationService.GetModulos(request.HeaderAuth);
                    response.Data = moduloResponse.Data;
                    response.Success = moduloResponse.Success;
                    response.Messages = moduloResponse.Messages;

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
