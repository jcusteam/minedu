using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobantePago.Application.Query.Dtos;
using RecaudacionApiComprobantePago.DataAccess;
using RecaudacionApiComprobantePago.Domain;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Application.Query
{
    public class FindByFuenteComprobantePagoHandler
    {
        public class StatusFindFuenteResponse : StatusResponse<ComprobantePagoDto>
        {

        }
        public class Query : IRequest<StatusFindFuenteResponse>
        {
            public ComprobantePagoFuenteDto FuenteDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindFuenteResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IMapper _mapper;

            public Handler(IComprobantePagoRepository repository,
                IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;

            }

            public async Task<StatusFindFuenteResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindFuenteResponse();

                try
                {
                    var fuente = request.FuenteDto;
                    var comprobantePagoFuente = new ComprobantePago();
                    comprobantePagoFuente.UnidadEjecutoraId = fuente.UnidadEjecutoraId;
                    comprobantePagoFuente.TipoComprobanteId = fuente.TipoComprobanteId;
                    comprobantePagoFuente.Serie = fuente.Serie;
                    comprobantePagoFuente.Correlativo = fuente.Correlativo;

                    var comprobantePago = await _repository.FindByFuente(comprobantePagoFuente);

                    if (comprobantePago == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.COMPROBANTE_INFO_NOT_EXISTS_DATA_DOCUMENTO_FUENTE));
                        response.Success = false;
                        return response;
                    }

                    response.Data = _mapper.Map<ComprobantePago, ComprobantePagoDto>(comprobantePago);

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
