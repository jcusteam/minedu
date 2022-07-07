using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiLiquidacion.Application.Query.Dtos;
using RecaudacionApiLiquidacion.DataAccess;
using MediatR;
using RecaudacionApiLiquidacion.Domain;
using RecaudacionApiLiquidacion.Clients;
using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.Application.Query
{
    public class PageLiquidacionHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public LiquidacionFilterDto LiquidacionFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly ILiquidacionRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IFuenteFinanciamientoAPI _fuenteFinanciamientoAPI;
            private readonly IMapper _mapper;

            public Handler(ILiquidacionRepository repository,
                IEstadoAPI estadoAPI,
                IFuenteFinanciamientoAPI fuenteFinanciamientoAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
                _estadoAPI = estadoAPI;
                _fuenteFinanciamientoAPI = fuenteFinanciamientoAPI;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusPageResponse();
                try
                {
                    var filter = _mapper.Map<LiquidacionFilter>(request.LiquidacionFilterDto);
                    var pagination = await _repository.FindPage(filter);

                    foreach (var item in pagination.Items)
                    {
                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(item.TipoDocumentoId, item.Estado);

                        if (estadoResponse.Success)
                        {
                            item.EstadoNombre = estadoResponse.Data.Nombre;
                        }

                        var fuenteFinaciamientoResponse = await _fuenteFinanciamientoAPI.FindByIdAsync((int)item.FuenteFinanciamientoId);

                        if (fuenteFinaciamientoResponse.Success)
                        {
                            item.FuenteFinanciamiento = fuenteFinaciamientoResponse.Data;
                        }
                    }
                    response.Data = _mapper.Map<Pagination<LiquidacionDto>>(pagination);
                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
