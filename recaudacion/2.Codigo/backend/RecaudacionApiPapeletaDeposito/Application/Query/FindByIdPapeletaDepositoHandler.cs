using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiPapeletaDeposito.Application.Query.Dtos;
using RecaudacionApiPapeletaDeposito.DataAccess;
using RecaudacionApiPapeletaDeposito.Domain;
using MediatR;
using RecaudacionApiPapeletaDeposito.Clients;
using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.Application.Query
{
    public class FindByIdPapeletaDepositoHandler
    {
        public class StatusFindResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindResponse>
        {
            private readonly IPapeletaDepositoRepository _repository;
            private readonly IPapeletaDepositoDetalleRepository _detalleRepository;
            private readonly IReciboIngresoAPI _reciboIngresoAPI;
            private readonly ITipoReciboIngresoAPI _tipoReciboIngresoAPI;
            private readonly ITipoCaptacionAPI _tipoCaptacionAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IBancoAPI _bancoAPI;
            private readonly IMapper _mapper;

            public Handler(
                IPapeletaDepositoRepository repository,
                ITipoReciboIngresoAPI tipoReciboIngresoAPI,
                IPapeletaDepositoDetalleRepository detalleRepository,
                IReciboIngresoAPI reciboIngresoAPI,
                ITipoCaptacionAPI tipoCaptacionAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IBancoAPI bancoAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _reciboIngresoAPI = reciboIngresoAPI;
                _tipoReciboIngresoAPI = tipoReciboIngresoAPI;
                _tipoCaptacionAPI = tipoCaptacionAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _bancoAPI = bancoAPI;
                _mapper = mapper;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var papeletaDeposito = await _repository.FindById(request.Id);

                    if (papeletaDeposito == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_ITEMS));
                        response.Success = false;
                        return response;
                    }

                    var detalles = await _detalleRepository.FindAll(papeletaDeposito.PapeletaDepositoId);

                    foreach (var item in detalles)
                    {
                        var reciboIngresoResponse = await _reciboIngresoAPI.FindByIdAsync(item.ReciboIngresoId);
                        if (reciboIngresoResponse.Success)
                        {
                            item.ReciboIngreso = reciboIngresoResponse.Data;

                            var tipoResponse = await _tipoReciboIngresoAPI.FindByIdAsync(item.ReciboIngreso.TipoReciboIngresoId);
                            if (tipoResponse.Success)
                            {
                                item.ReciboIngreso.TipoReciboIngreso = tipoResponse.Data;
                            }

                            var cuentaCorrienteResponse = await _cuentaCorrienteAPI.FindByIdAsync(item.ReciboIngreso.CuentaCorrienteId);
                            if (cuentaCorrienteResponse.Success)
                            {
                                item.ReciboIngreso.CuentaCorriente = cuentaCorrienteResponse.Data;

                                var bancoResponse = await _bancoAPI.FindByIdAsync(item.ReciboIngreso.CuentaCorriente.BancoId);

                                if (bancoResponse.Success)
                                {
                                    item.ReciboIngreso.CuentaCorriente.Banco = bancoResponse.Data;
                                }
                            }

                            var tipoCaptacionResponse = await _tipoCaptacionAPI.FindByIdAsync(item.ReciboIngreso.TipoCaptacionId);

                            if (tipoCaptacionResponse.Success)
                            {
                                item.ReciboIngreso.TipoCaptacion = tipoCaptacionResponse.Data;
                            }
                        }
                    }

                    var papeletaDepositoDto = _mapper.Map<PapeletaDeposito, PapeletaDepositoDto>(papeletaDeposito);
                    papeletaDepositoDto.PapeletaDepositoDetalle = _mapper.Map<List<PapeletaDepositoDetalleDto>>(detalles);
                    response.Data = papeletaDepositoDto;

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
