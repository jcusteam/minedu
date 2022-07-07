using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiDepositoBanco.Application.Query.Dtos;
using RecaudacionApiDepositoBanco.DataAccess;
using RecaudacionApiDepositoBanco.Domain;
using MediatR;
using RecaudacionUtils;
using RecaudacionApiDepositoBanco.Clients;

namespace RecaudacionApiDepositoBanco.Application.Query
{
    public class FindByNumeroDepositoBancoDetalleHandler
    {
        public class StatusFindNumeroDetalleResponse : StatusResponse<Object>
        {
        }
        public class Query : IRequest<StatusFindNumeroDetalleResponse>
        {
            public string NumerDeposito { get; set; }
            public DateTime FechaDeposito { get; set; }
            public int CuentaCorrienteId { get; set; }
            public int ClienteId { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindNumeroDetalleResponse>
        {

            private readonly IDepositoBancoDetalleReposiory _detalleReposiory;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IMapper _mapper;

            public Handler(IDepositoBancoDetalleReposiory detalleReposiory,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IMapper mapper)
            {
                _detalleReposiory = detalleReposiory;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _mapper = mapper;
            }

            public async Task<StatusFindNumeroDetalleResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindNumeroDetalleResponse();

                try
                {
                    var numero = request.NumerDeposito;
                    var fecha = request.FechaDeposito;
                    var cuentaCorrienteId = request.CuentaCorrienteId;
                    var clienteId = request.ClienteId;
                    var depositoBancoDetalle = await _detalleReposiory.FindByNumeroAndFechaAndCuentaCorriente(numero, fecha, cuentaCorrienteId, clienteId);

                    if (depositoBancoDetalle == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número de depósito '{numero}' no se encuentra registrado"));
                        response.Success = false;
                        return response;
                    }

                    var tipoDocResponse = await _tipoDocumentoAPI.FindByIdAsync(depositoBancoDetalle.TipoDocumento ?? 0);
                    if (tipoDocResponse.Success)
                    {
                        depositoBancoDetalle.TipoDocumentoNombre = tipoDocResponse.Data.Abreviatura;
                    }

                    response.Data = _mapper.Map<DepositoBancoDetalle, DepositoBancoDetalleDto>(depositoBancoDetalle);

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
