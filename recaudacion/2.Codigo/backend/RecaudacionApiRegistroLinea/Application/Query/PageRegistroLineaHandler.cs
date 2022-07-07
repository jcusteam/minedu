using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiRegistroLinea.Application.Query.Dtos;
using RecaudacionApiRegistroLinea.DataAccess;
using MediatR;
using RecaudacionApiRegistroLinea.Domain;
using RecaudacionApiRegistroLinea.Clients;
using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.Application.Query
{
    public class PageRegistroLineaHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {

        }
        public class Query : IRequest<StatusPageResponse>
        {
            public RegistroLineaFilterDto RegistroLineaFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IRegistroLineaRepository _repository;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;

            public Handler(IRegistroLineaRepository repository,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IClienteAPI clienteAPI,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IBancoAPI bancoAPI,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _clienteAPI = clienteAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _bancoAPI = bancoAPI;
                _repository = repository;
                _estadoAPI = estadoAPI;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<RegistroLineaFilter>(request.RegistroLineaFilterDto);


                    if (!String.IsNullOrEmpty(filter.Rol))
                    {
                        switch (filter.Rol)
                        {
                            case Definition.ROLE_OT_JEFE:
                                filter.Estados = $"{Definition.REGISTRO_LINEA_ESTADO_EN_PROCESO}";
                                break;
                            case Definition.ROLE_VENTANILLA:
                                filter.Estados = $"{Definition.REGISTRO_LINEA_ESTADO_EMITIDO}";
                                break;
                            case Definition.ROLE_TEC_ADMIN:
                                // code block
                                break;
                            case Definition.ROLE_REGISTRO_SIAF:
                                filter.Estados = $"{Definition.REGISTRO_LINEA_ESTADO_DERIVADO},{Definition.REGISTRO_LINEA_ESTADO_DESESTIMADO},{Definition.REGISTRO_LINEA_ESTADO_EMITIR_RI}";
                                break;
                            case Definition.ROLE_COORDINADOR:
                                filter.Estados = $"{Definition.REGISTRO_LINEA_ESTADO_EMITIDO},{Definition.REGISTRO_LINEA_ESTADO_EMITIR_RI}";
                                break;
                            case Definition.ROLE_GIRO_PAGO:
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

                        var bancoResponse = await _bancoAPI.FindByIdAsync(item.BancoId);
                        if (bancoResponse.Success)
                        {
                            item.Banco = bancoResponse.Data;
                        }

                        var cuentaCorrienteResponse = await _cuentaCorrienteAPI.FindByIdAsync(item.CuentaCorrienteId);
                        if (cuentaCorrienteResponse.Success)
                        {
                            item.CuentaCorriente = cuentaCorrienteResponse.Data;
                        }

                        var clienteResponse = await _clienteAPI.FindByIdAsync(item.ClienteId);
                        if (clienteResponse.Success)
                        {
                            item.Cliente = clienteResponse.Data;
                        }

                        var tipoReciboResponse = await _tipoReciboIngresoAPI.FindByIdAsync(item.TipoReciboIngresoId);
                        if (tipoReciboResponse.Success)
                        {
                            item.TipoReciboIngreso = tipoReciboResponse.Data;
                        }

                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(item.TipoDocumentoId, item.Estado);
                        if (estadoResponse.Success)
                        {
                            item.EstadoNombre = estadoResponse.Data.Nombre;
                        }
                    }

                    response.Data = _mapper.Map<Pagination<RegistroLineaDto>>(pagination);
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
