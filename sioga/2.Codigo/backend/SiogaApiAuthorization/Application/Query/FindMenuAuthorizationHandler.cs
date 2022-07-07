using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Application.Query.Dtos;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;
using FluentValidation;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindMenuAuthorizationHandler
    {
        public class StatusMenuResponse : StatusApiResponse<object>
        {

        }
        public class Query : IRequest<StatusMenuResponse>
        {
            public AuthMenuDto AuthDto { get; set; }
            public string HeaderAuth { get; set; }
        }

        public class CommandValidator : AbstractValidator<Query>
        {
            public CommandValidator()
            {
                RuleFor(x => x.HeaderAuth)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Authorization Bearer es requerido");

                RuleFor(x => x.AuthDto.CodigoModulo)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Codigo Modulo es requerido");
            }
        }

        public class Handler : IRequestHandler<Query, StatusMenuResponse>
        {
            private readonly IAutorizationService _autorizationService;

            public Handler(IAutorizationService autorizationService
            )
            {
                _autorizationService = autorizationService;
            }

            public async Task<StatusMenuResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusMenuResponse();

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

                    var menuResponse = await _autorizationService.GetMenus(request.AuthDto.CodigoModulo, request.HeaderAuth);
                    response.Data = menuResponse.Data;
                    response.Success = menuResponse.Success;
                    response.Messages = menuResponse.Messages;

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
