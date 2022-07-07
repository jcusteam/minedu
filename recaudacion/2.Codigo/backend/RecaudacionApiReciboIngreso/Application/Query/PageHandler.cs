using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiReciboIngreso.Application.Query.Dtos;
using RecaudacionApiReciboIngreso.DataAccess;
using MediatR;
using RecaudacionApiReciboIngreso.Domain;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.Application.Query
{
    public class PageHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public ReciboIngresoFilterDto ReciboIngresoFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly IMapper _mapper;
            public Handler(IReciboIngresoRepository repository,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IClienteAPI clienteAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IEstadoAPI estadoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _clienteAPI = clienteAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _estadoAPI = estadoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<ReciboIngresoFilter>(request.ReciboIngresoFilterDto);

                    if (String.IsNullOrEmpty(filter.Rol))
                    {
                        switch (filter.Rol)
                        {
                            case Definition.ROLE_OT_JEFE:
                                filter.Estados = $"{Definition.RECIBO_INGRESO_ESTADO_EMITIDO,Definition.RECIBO_INGRESO_ESTADO_TRANSMITIDO}";
                                break;
                            case Definition.ROLE_COORDINADOR:
                                // code block
                                break;
                            default:
                                // code block
                                break;
                        }
                    }

                    var pagination = await _repository.FindPage(filter);

                    foreach (var item in pagination.Items)
                    {

                        var clienteResponse = await _clienteAPI.FindByIdAsync(item.ClienteId);
                        if (clienteResponse.Success)
                        {
                            item.Cliente = clienteResponse.Data;
                        }

                        var tipoResponse = await _tipoReciboIngresoAPI.FindByIdAsync(item.TipoReciboIngresoId);
                        if (tipoResponse.Success)
                        {
                            item.TipoReciboIngreso = tipoResponse.Data;
                        }

                        var cuentaCorrienteResponse = await _cuentaCorrienteAPI.FindByIdAsync(item.CuentaCorrienteId);
                        if (cuentaCorrienteResponse.Success)
                        {
                            item.CuentaCorriente = cuentaCorrienteResponse.Data;
                        }

                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(
                           item.TipoDocumentoId, item.Estado
                       );
                        if (estadoResponse.Success)
                        {
                            item.EstadoNombre = estadoResponse.Data.Nombre;
                        }

                        var tipoCaptacionResponse = await _tipoCaptacionAPI.FindByIdAsync(item.TipoCaptacionId);

                        if (tipoCaptacionResponse.Success)
                        {
                            item.TipoCaptacion = tipoCaptacionResponse.Data;
                        }
                    }

                    response.Data = _mapper.Map<Pagination<ReciboIngresoDto>>(pagination);
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
