using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaUtils;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindStoreAuthorizationHandler
    {
        public class StatusStoreResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusStoreResponse>
        {
            public string Key { get; set; }
        }

        public class CommandValidator : AbstractValidator<Query>
        {
            public CommandValidator()
            {

            }
        }

        public class Handler : IRequestHandler<Query, StatusStoreResponse>
        {
            private readonly IAuthStore _authStore;

            public Handler(IAuthStore authStore
            )
            {
                _authStore = authStore;
            }

            public async Task<StatusStoreResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusStoreResponse();
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

                    response.Data = await _authStore.GetStore(request.Key);
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
