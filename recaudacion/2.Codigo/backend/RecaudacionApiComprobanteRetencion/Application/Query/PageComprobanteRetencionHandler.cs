using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobanteRetencion.Application.Query.Dtos;
using RecaudacionApiComprobanteRetencion.DataAccess;
using MediatR;
using RecaudacionApiComprobanteRetencion.Domain;
using RecaudacionApiComprobanteRetencion.Clients;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteRetencion.Application.Query
{
    public class PageComprobanteRetencionHandler
    {

        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public ComprobanteRetencionFilterDto ComprobanteRetencionFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IComprobanteRetencionRepository _repository;
            private readonly IClienteAPI _clienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IComprobanteRetencionRepository repository,
                IClienteAPI clienteAPI,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _clienteAPI = clienteAPI;
                _estadoAPI = estadoAPI;
                _mapper = mapper;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<ComprobanteRetencionFilter>(request.ComprobanteRetencionFilterDto);
                    var pagination = await _repository.FindPage(filter);

                    foreach (var item in pagination.Items)
                    {
                        var clienteResponse = await _clienteAPI.FindByIdAsync(item.ClienteId);

                        if (clienteResponse.Success)
                        {
                            item.Cliente = clienteResponse.Data;
                        }

                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(item.TipoComprobanteId, item.Estado);
                        if (estadoResponse.Success)
                        {
                            item.NombreEstado = estadoResponse.Data.Nombre;
                        }

                        if (!String.IsNullOrEmpty(item.RegimenRetencion))
                        {
                            if (item.RegimenRetencion == Definition.TIPO_REGIMEN_RETENCION_01)
                            {
                                item.RegimenRetencionDesc = Definition.TIPO_REGIMEN_RETENCION_TASA_3;
                            }
                            else
                            {
                                item.RegimenRetencionDesc = Definition.TIPO_REGIMEN_RETENCION_TASA_6;
                            }
                        }

                    }

                    response.Data = _mapper.Map<Pagination<ComprobanteRetencionDto>>(pagination);

                }
                catch (Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
