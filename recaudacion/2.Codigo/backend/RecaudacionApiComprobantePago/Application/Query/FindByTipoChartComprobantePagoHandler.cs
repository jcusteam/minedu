using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobantePago.DataAccess;
using RecaudacionApiComprobantePago.Domain;
using MediatR;
using RecaudacionApiComprobantePago.Clients;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Application.Query
{
    public class FindByTipoChartComprobantePagoHandler
    {


        public class StatusChartResponse : StatusResponse<List<ChartTipo>>
        {
        }
        public class Query : IRequest<StatusChartResponse>
        {
            public int EjecutoraId { get; set; }
            public int Anio { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusChartResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly ITipoComprobantePagoAPI _tipoComprobantePagoAPI;
            private readonly IMapper _mapper;

            public Handler(IComprobantePagoRepository repository,
               ITipoComprobantePagoAPI tipoComprobantePagoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _tipoComprobantePagoAPI = tipoComprobantePagoAPI;
                _mapper = mapper;

            }

            public async Task<StatusChartResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusChartResponse();

                try
                {
                    var tipoComprobantePagoResponse = await _tipoComprobantePagoAPI.FindAllAsync();

                    List<ChartTipo> lista = new List<ChartTipo>();

                    if (tipoComprobantePagoResponse.Success)
                    {
                        foreach (var item in tipoComprobantePagoResponse.Data)
                        {
                            var chartTipoRecibo = new ChartTipo();
                            chartTipoRecibo.TipoId = item.TipoComprobantePagoId;
                            chartTipoRecibo.TipoName = item.Nombre;
                            lista.Add(chartTipoRecibo);
                        }

                        foreach (var item in lista)
                        {
                            var charts = await _repository.FindChartByTipo(item.TipoId, request.EjecutoraId, request.Anio);
                            item.charts = charts;
                        }
                        response.Data = lista;
                        response.Success = true;
                    }
                    else
                    {

                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
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
