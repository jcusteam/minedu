using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobanteRetencion.Application.Query.Dtos;
using RecaudacionApiComprobanteRetencion.DataAccess;
using RecaudacionApiComprobanteRetencion.Domain;
using MediatR;
using RecaudacionUtils;
using RecaudacionApiComprobanteRetencion.Clients;

namespace RecaudacionApiComprobanteRetencion.Application.Query
{
    public class FindByIdComprobanteRetencionHandler
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
            private readonly IComprobanteRetencionRepository _repository;
            private readonly IComprobanteRetencionDetalleRepository _detalleRepository;
            private readonly ITipoComprobantePagoAPI _tipoComprobantePagoAPI;
            private readonly IMapper _mapper;
            public Handler(
                IComprobanteRetencionRepository repository,
                IComprobanteRetencionDetalleRepository detalleRepository,
                ITipoComprobantePagoAPI tipoComprobantePagoAPI,
                 IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _tipoComprobantePagoAPI = tipoComprobantePagoAPI;
                _mapper = mapper;
            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var comprobanteRetencion = await _repository.FindById(request.Id);

                    if (comprobanteRetencion == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    var detalles = await _detalleRepository.FindAll(comprobanteRetencion.ComprobanteRetencionId);

                    foreach (var item in detalles)
                    {
                        var tipoComprobanteResponse = await _tipoComprobantePagoAPI.FindByCodigoAsync(item.TipoDocumento);
                        if (tipoComprobanteResponse.Success)
                        {
                            item.TipoDocumentoNombre = tipoComprobanteResponse.Data.Nombre;
                        }
                    }

                    var comprobanteRetencionDto = _mapper.Map<ComprobanteRetencion, ComprobanteRetencionDto>(comprobanteRetencion);
                    comprobanteRetencionDto.ComprobanteRetencionDetalle = _mapper.Map<List<ComprobanteRetencionDetalleDto>>(detalles);
                    response.Data = comprobanteRetencionDto;

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
