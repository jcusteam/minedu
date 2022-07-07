using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ESinadApiExpediente.DataAccess;
using SiogaUtils;

namespace ESinadApiExpediente.Application.Query
{
    public class FindByExpedienteNumeroHandler
    {
        public class StatusFindExpedienteResponse : StatusApiResponse<object>
        {

        }

        public class Query : IRequest<StatusFindExpedienteResponse>
        {
            public string NumeroExpediente { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindExpedienteResponse>
        {
            private readonly IExpedienteRepository _repository;
            public Handler(IExpedienteRepository repository)
            {
                _repository = repository;
            }

            public async Task<StatusFindExpedienteResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindExpedienteResponse();

                try
                {
                    var sinad = await _repository.FindByNumeroExpediente(request.NumeroExpediente);

                    if (sinad == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "No se ha podido obtener el registro"));
                        response.Success = false;
                    }
                    else
                    {
                        response.Data = sinad;
                        response.Success = true;
                    }

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
