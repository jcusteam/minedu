using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiPapeletaDeposito.Application.Query.Dtos;
using RecaudacionApiPapeletaDeposito.DataAccess;
using MediatR;
using RecaudacionApiPapeletaDeposito.Domain;
using RecaudacionApiPapeletaDeposito.Clients;
using Microsoft.Extensions.Logging;
using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.Application.Query
{
    public class PagePapeletaDepositoHandler
    {

        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public PapeletaDepositoFilterDto PapeletaDepositoFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IPapeletaDepositoRepository _repository;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            private readonly ILogger logger;
            public Handler(
                ILogger<Handler> logger,
                IPapeletaDepositoRepository repository,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IBancoAPI bancoAPI,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _bancoAPI = bancoAPI;
                _repository = repository;
                _estadoAPI = estadoAPI;
                this.logger = logger;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {


                    var filter = _mapper.Map<PapeletaDepositoFilter>(request.PapeletaDepositoFilterDto);
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

                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(item.TipoDocumentoId, item.Estado);
                        if (estadoResponse.Success)
                        {
                            item.EstadoNombre = estadoResponse.Data.Nombre;
                        }
                    }

                    response.Data = _mapper.Map<Pagination<PapeletaDepositoDto>>(pagination);
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
