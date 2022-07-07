using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobantePago.Application.Query.Dtos;
using RecaudacionApiComprobantePago.DataAccess;
using MediatR;
using RecaudacionApiComprobantePago.Domain;
using RecaudacionApiComprobantePago.Clients;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Application.Query
{
    public class PageComprobantePagoHandler
    {

        public class StatusPageResponse : StatusResponse<Pagination<ComprobantePagoDto>>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public ComprobantePagoFilterDto ComprobantePagoFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IClienteAPI _clienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IComprobantePagoRepository repository,
            IClienteAPI clienteAPI,
            IEstadoAPI estadoAPI,
            IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
                _clienteAPI = clienteAPI;
                _estadoAPI = estadoAPI;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<ComprobantePagoFilter>(request.ComprobantePagoFilterDto);

                    var pagination = await _repository.FindPage(filter);

                    foreach (var item in pagination.Items)
                    {
                        var clienteResponse = await _clienteAPI.FindByIdAsync(item.ClienteId);

                        if (clienteResponse.Success)
                        {
                            item.Cliente = clienteResponse.Data;
                        }

                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(item.TipoDocumentoId, item.Estado);
                        if (estadoResponse.Success)
                        {
                            item.NombreEstado = estadoResponse.Data.Nombre;
                        }
                    }

                    response.Data = _mapper.Map<Pagination<ComprobantePagoDto>>(pagination);
                    response.Success = true;

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
