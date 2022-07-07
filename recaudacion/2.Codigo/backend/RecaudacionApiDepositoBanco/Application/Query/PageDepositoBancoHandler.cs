using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiDepositoBanco.Application.Query.Dtos;
using RecaudacionApiDepositoBanco.DataAccess;
using MediatR;
using RecaudacionApiDepositoBanco.Domain;
using RecaudacionApiDepositoBanco.Clients;
using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.Application.Query
{
    public class PageDepositoBancoHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {

        }
        public class Query : IRequest<StatusPageResponse>
        {
            public DepositoBancoFilterDto DepositoBancoFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IDepositoBancoRepository _repository;
            private readonly IBancoAPI _bancoAPI;
            private readonly ICuentaCorrienteAPI _cuentaCorrienteAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IMapper _mapper;
            public Handler(IDepositoBancoRepository repository,
                IBancoAPI bancoAPI,
                ICuentaCorrienteAPI cuentaCorrienteAPI,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
                _bancoAPI = bancoAPI;
                _cuentaCorrienteAPI = cuentaCorrienteAPI;
                _estadoAPI = estadoAPI;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<DepositoBancoFilter>(request.DepositoBancoFilterDto);
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

                    response.Data = _mapper.Map<Pagination<DepositoBancoDto>>(pagination);

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
