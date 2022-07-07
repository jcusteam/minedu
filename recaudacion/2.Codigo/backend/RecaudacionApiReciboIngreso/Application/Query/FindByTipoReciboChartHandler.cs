using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiReciboIngreso.DataAccess;
using MediatR;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;
using RecaudacionApiReciboIngreso.Application.Query.Dtos;

namespace RecaudacionApiReciboIngreso.Application.Query
{
    public class FindByTipoReciboChartHandler
    {
        public class StatusChartTipoReciboResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusChartTipoReciboResponse>
        {
            public ChartFilterDto FilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusChartTipoReciboResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;

            public Handler(IReciboIngresoRepository repository,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI)
            {
                _repository = repository;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
            }

            public async Task<StatusChartTipoReciboResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusChartTipoReciboResponse();

                try
                {
                    var tipoRecibosResponse = await _tipoReciboIngresoAPI.FindAllAsync();

                    if (!tipoRecibosResponse.Success)
                    {
                        response.Messages.Add(new GenericMessage(tipoRecibosResponse.Messages[0].Type, $"Servicio de Tipo Recibo Ingreso: {tipoRecibosResponse.Messages[0].Message} "));
                        response.Success = false;
                        return response;
                    }

                    var filter = request.FilterDto;

                    var items = await _repository.FindChart(tipoRecibosResponse.Data, filter.UnidadEjecutoraId, filter.Anio);
                    response.Data = items;
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
