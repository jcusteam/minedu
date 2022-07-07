using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiReciboIngreso.Application.Query.Dtos;
using RecaudacionApiReciboIngreso.DataAccess;
using RecaudacionApiReciboIngreso.Domain;
using MediatR;
using RecaudacionApiReciboIngreso.Clients;
using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.Application.Query
{
    public class FindByNumeroEjecutoraCuentaHandler
    {
        public class StatusFindNumeroEjecutoraResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindNumeroEjecutoraResponse>
        {
            public string Numero { get; set; }
            public int UnidadEjecutoraId { get; set; }
            public int CuentaCorrienteId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindNumeroEjecutoraResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
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
                _tipoCaptacionAPI = tipoCaptacionAPI;
            }
            public async Task<StatusFindNumeroEjecutoraResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindNumeroEjecutoraResponse();

                try
                {
                    var reciboIngreso = await _repository.FindByNumeroAndEjecutoraAndCuenta(request.Numero, request.UnidadEjecutoraId, request.CuentaCorrienteId);

                    if (reciboIngreso == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    var clienteResponse = await _clienteAPI.FindByIdAsync(reciboIngreso.ClienteId);
                    if (clienteResponse.Success)
                    {
                        reciboIngreso.Cliente = clienteResponse.Data;
                    }

                    var tipoResponse = await _tipoReciboIngresoAPI.FindByIdAsync(reciboIngreso.TipoReciboIngresoId);
                    if (tipoResponse.Success)
                    {
                        reciboIngreso.TipoReciboIngreso = tipoResponse.Data;
                    }

                    var cuentaCorrienteResponse = await _cuentaCorrienteAPI.FindByIdAsync(reciboIngreso.CuentaCorrienteId);
                    if (cuentaCorrienteResponse.Success)
                    {
                        reciboIngreso.CuentaCorriente = cuentaCorrienteResponse.Data;
                    }

                    var tipoCaptacionResponse = await _tipoCaptacionAPI.FindByIdAsync(reciboIngreso.TipoCaptacionId);

                    if (tipoCaptacionResponse.Success)
                    {
                        reciboIngreso.TipoCaptacion = tipoCaptacionResponse.Data;
                    }

                    response.Data = _mapper.Map<ReciboIngreso, ReciboIngresoDto>(reciboIngreso);

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
