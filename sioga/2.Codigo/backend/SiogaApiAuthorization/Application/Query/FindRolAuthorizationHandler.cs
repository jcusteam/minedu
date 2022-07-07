using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;
using FluentValidation;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindRolAuthorizationHandler
    {
        public class StatusRolResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusRolResponse>
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

        public class Handler : IRequestHandler<Query, StatusRolResponse>
        {
            private readonly IAutorizationService _autorizationService;

            public Handler(IAutorizationService autorizationService
            )
            {
                _autorizationService = autorizationService;
            }

            public async Task<StatusRolResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusRolResponse();

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

                    var rolResponse = await _autorizationService.GetRoles(request.HeaderAuth);
                    response.Data = rolResponse.Data;
                    response.Success = rolResponse.Success;
                    response.Messages = rolResponse.Messages;
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
