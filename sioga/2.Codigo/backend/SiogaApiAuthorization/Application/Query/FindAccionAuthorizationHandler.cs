using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaApiAuthorization.Application.Query.Dtos;
using SiogaUtils;
using FluentValidation;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindAccionAuthorizationHandler
    {
        public class StatusAccionResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusAccionResponse>
        {
            public AuthAccionDto AuthDto { get; set; }
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

                RuleFor(x => x.AuthDto.CodigoMenu)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Codigo Menu es requerido");
            }
        }


        public class Handler : IRequestHandler<Query, StatusAccionResponse>
        {
            private readonly IAutorizationService _autorizationService;

            public Handler(IAutorizationService autorizationService)
            {
                _autorizationService = autorizationService;
            }

            public async Task<StatusAccionResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusAccionResponse();

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

                    var auth = request.AuthDto;
                    var accionResponse = await _autorizationService.GetAcciones(auth.CodigoModulo, auth.CodigoMenu, request.HeaderAuth);

                    if (!accionResponse.Success)
                    {
                        response.Success = accionResponse.Success;
                        response.Messages = accionResponse.Messages;
                        return response;
                    }

                    if (accionResponse.Data.Count == 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_ITEMS));
                        response.Success = false;
                        return response;
                    }

                    response.Data = accionResponse.Data;

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
