using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiComprobantePago.Application.Query.Dtos;
using RecaudacionApiComprobantePago.DataAccess;
using RecaudacionApiComprobantePago.Domain;
using MediatR;
using RecaudacionApiComprobantePago.Clients;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Application.Query
{
    public class FindByIdComprobantePagoHandler
    {
        public class StatusFindResponse : StatusResponse<ComprobantePagoDto>
        {

        }
        public class Query : IRequest<StatusFindResponse>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusFindResponse>
        {
            private readonly IComprobantePagoRepository _repository;
            private readonly IComprobantePagoDetalleRepository _detalleRepository;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly ITarifarioAPI _tarifarioAPI;
            private readonly IMapper _mapper;

            public Handler(IComprobantePagoRepository repository,
                IComprobantePagoDetalleRepository detalleRepository,
                ICatalogoBienAPI catalogoBienAPI,
                ITarifarioAPI tarifarioAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _catalogoBienAPI = catalogoBienAPI;
                _tarifarioAPI = tarifarioAPI;
                _mapper = mapper;

            }

            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var comprobantePago = await _repository.FindById(request.Id);

                    if (comprobantePago == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                        return response;
                    }

                    var comprobantePagoDetalles = await _detalleRepository.FindAll(comprobantePago.ComprobantePagoId);
                    foreach (var item in comprobantePagoDetalles)
                    {
                        if (item.TarifarioId > 0)
                        {
                            var tarifarioResponse = await _tarifarioAPI.FindByIdAsync(item.TarifarioId ?? 0);

                            if (tarifarioResponse.Success)
                            {
                                item.Tarifario = tarifarioResponse.Data;
                            }
                        }
                        if (item.CatalogoBienId > 0)
                        {
                            var catalogoBienResponse = await _catalogoBienAPI.FindByIdAsync(item.CatalogoBienId ?? 0);

                            if (catalogoBienResponse.Success)
                            {
                                item.CatalogoBien = catalogoBienResponse.Data;
                            }
                        }
                    }

                    var comprobantePagoDto = _mapper.Map<ComprobantePago, ComprobantePagoDto>(comprobantePago);
                    comprobantePagoDto.ComprobantePagoDetalle = _mapper.Map<List<ComprobantePagoDetalleDto>>(comprobantePagoDetalles);
                    response.Data = comprobantePagoDto;

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
