using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiDepositoBanco.Application.Query.Dtos;
using RecaudacionApiDepositoBanco.DataAccess;
using RecaudacionApiDepositoBanco.Domain;
using MediatR;
using RecaudacionApiDepositoBanco.Clients;
using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.Application.Query
{
    public class FindByIdDepositoBancoHandler
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
            private readonly IDepositoBancoRepository _repository;
            private readonly IDepositoBancoDetalleReposiory _detalleReposiory;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IClienteAPI _clienteAPI;
            private readonly IMapper _mapper;

            public Handler(IDepositoBancoRepository repository,
                IDepositoBancoDetalleReposiory detalleReposiory,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IClienteAPI clienteAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleReposiory = detalleReposiory;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _clienteAPI = clienteAPI;
                _mapper = mapper;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var depositoBanco = await _repository.FindById(request.Id);

                    if (depositoBanco == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    var items = await _detalleReposiory.FindAll(depositoBanco.DepositoBancoId);

                    foreach (var item in items)
                    {
                        var clienteResponse = await _clienteAPI.FindByIdAsync(item.ClienteId);
                        if (clienteResponse.Success)
                        {
                            item.Cliente = clienteResponse.Data;
                        }
                        item.TipoDocumentoNombre = "";
                        var tipoDocResponse = await _tipoDocumentoAPI.FindByIdAsync(item.TipoDocumento ?? 0);
                        if (tipoDocResponse.Success)
                        {
                            item.TipoDocumentoNombre = tipoDocResponse.Data.Abreviatura;
                        }

                    }

                    var depositoBancoDto = _mapper.Map<DepositoBanco, DepositoBancoDto>(depositoBanco);
                    depositoBancoDto.DepositoBancoDetalle = _mapper.Map<List<DepositoBancoDetalleDto>>(items);

                    response.Data = depositoBancoDto;


                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
