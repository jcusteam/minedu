using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.DataAccess;
using MediatR;
using RecaudacionUtils;
using RecaudacionApiReciboIngreso.Clients;

namespace RecaudacionApiReciboIngreso.Application.Command
{
    public class DeleteHandler
    {
        public class StatusDeleteResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusDeleteResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, StatusDeleteResponse>
        {
            private readonly IReciboIngresoRepository _repository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IDepositoBancoAPI _depositoBancoAPI;

            public Handler(IReciboIngresoRepository repository,
                IEstadoAPI estadoAPI,
                IDepositoBancoAPI depositoBancoAPI)
            {
                _repository = repository;
                _estadoAPI = estadoAPI;
                _depositoBancoAPI = depositoBancoAPI;
            }

            public async Task<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new StatusDeleteResponse();
                try
                {
                    var reciboIngreso = await _repository.FindById(request.Id);
                    if (reciboIngreso == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_PROCESS));
                        response.Success = false;
                        return response;
                    }

                    if (reciboIngreso.Estado != Definition.RECIBO_INGRESO_ESTADO_EMITIDO)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, Message.WARNING_DELETE));
                        response.Success = false;
                        return response;
                    }

                    await _repository.Delete(reciboIngreso);

                    if (reciboIngreso.Estado == Definition.RECIBO_INGRESO_ESTADO_EMITIDO)
                    {
                        if (reciboIngreso.TipoCaptacionId == Definition.TIPO_CAPTACION_DEPOSITO_CUENTA)
                        {
                            var responseDepositoBancoDetalle = await _depositoBancoAPI.FindDetalleByIdAsync(reciboIngreso.DepositoBancoDetalleId ?? 0);

                            if (responseDepositoBancoDetalle.Success)
                            {
                                var depositoBancoDetalle = responseDepositoBancoDetalle.Data;
                                depositoBancoDetalle.TipoDocumento = null;
                                depositoBancoDetalle.NumeroDocumento = null;
                                depositoBancoDetalle.FechaDocumento = null;
                                depositoBancoDetalle.Utilizado = Definition.DEPOSITO_BANCO_DETALLE_UTILIZADO_NO;
                                depositoBancoDetalle.UsuarioModificador = reciboIngreso.UsuarioModificador;
                                await _depositoBancoAPI.UpdateDetalleAsync(depositoBancoDetalle.DepositoBancoDetalleId, depositoBancoDetalle);

                            }

                        }
                    }

                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_DELETE));

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
