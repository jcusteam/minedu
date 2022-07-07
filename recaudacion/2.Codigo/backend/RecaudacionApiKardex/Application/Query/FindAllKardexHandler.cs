using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiKardex.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiKardex.Application.Query
{
    public class FindAllKardexHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
            public int CatalogoBienId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IKardexRepository _repository;

            public Handler(IKardexRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusFindAllResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllResponse();

                try
                {
                    if (request.CatalogoBienId <= 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, $"El Id Catalogo Bien no pude ser {request.CatalogoBienId}"));
                        response.Success = false;
                        return response;
                    }

                    var kardexs = await _repository.FindAll(request.CatalogoBienId);

                    if (kardexs.Count == 0)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_ITEMS));
                        response.Success = false;
                        return response;
                    }

                    response.Data = kardexs;

                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
