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
    public class FindByIdDepositoBancoDetalleHandler
    {
        public class StatusFindDetalleResponse : StatusResponse<object>
        {

        }
        public class Query : IRequest<StatusFindDetalleResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindDetalleResponse>
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

            public async Task<StatusFindDetalleResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindDetalleResponse();

                try
                {
                    var depositoBancoDetalle = await _detalleReposiory.FindById(request.Id);

                    if (depositoBancoDetalle == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
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
