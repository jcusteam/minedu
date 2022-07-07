using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RecaudacionUtils;
using RecaudacionApiPapeletaDeposito.Clients;
using RecaudacionApiPapeletaDeposito.Clients.Dtos;
using RecaudacionApiPapeletaDeposito.DataAccess;

namespace RecaudacionApiPapeletaDeposito.Application.Query
{
    public class FindByNroEjecutoraReciboIngresoHandler
    {
        public class StatusFindNroEjecutoraResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindNroEjecutoraResponse>
        {
            public ReciboIngresoConsultaDto ConsultaDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindNroEjecutoraResponse>
        {
            private readonly IPapeletaDepositoDetalleRepository _detalleRepository;
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            public Handler(IPapeletaDepositoDetalleRepository detalleRepository,
                IReciboIngresoAPI reciboIngresoAPI)
            {
                _detalleRepository = detalleRepository;
                _reciboIngresoAPI = reciboIngresoAPI;
            }
            public async Task<StatusFindNroEjecutoraResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindNroEjecutoraResponse();

                try
                {
                    var reciboIngresoResponse = await _reciboIngresoAPI.FindByNroEjecutoraAsync(request.ConsultaDto);

                    if (reciboIngresoResponse.Success)
                    {
                        if (reciboIngresoResponse.Data.Estado != Definition.RECIBO_INGRESO_ESTADO_TRANSMITIDO)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número '{request.ConsultaDto.Numero}' de recibo de ingreso debe estar en estado 'Transmitido'"));
                            response.Success = false;
                            return response;
                        }

                        //var papeletaDepositoDetalle = await _detalleRepository.FindByReciboIngreso(reciboIngresoResponse.Data.ReciboIngresoId);

                        //if (papeletaDepositoDetalle != null)
                        //{
                        //    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número '{request.ConsultaDto.Numero}' de recibo de ingreso ya se encuentra registrado"));
                        //    response.Success = false;
                        //    return response;
                        //}

                        response.Data = reciboIngresoResponse.Data;

                    }
                    response.Messages = reciboIngresoResponse.Messages;
                    response.Success = reciboIngresoResponse.Success;


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
