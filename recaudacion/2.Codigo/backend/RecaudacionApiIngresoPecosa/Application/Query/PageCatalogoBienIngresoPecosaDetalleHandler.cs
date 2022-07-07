using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiIngresoPecosa.Application.Query.Dtos;
using RecaudacionApiIngresoPecosa.DataAccess;
using MediatR;
using RecaudacionApiIngresoPecosa.Domain;
using RecaudacionApiIngresoPecosa.Clients;
using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.Application.Query
{
    public class PageCatalogoBienIngresoPecosaDetalleHandler
    {
        public class StatusPageCatalogoBienResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageCatalogoBienResponse>
        {
            public CatalogoBienFilterDto CatalogoBienFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageCatalogoBienResponse>
        {
            private readonly IIngresoPecosaDetalleRepository _detalleRepository;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly IMapper _mapper;

            public Handler(
                 IIngresoPecosaDetalleRepository detalleRepository,
                 ICatalogoBienAPI catalogoBienAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _detalleRepository = detalleRepository;
                _catalogoBienAPI = catalogoBienAPI;
            }
            public async Task<StatusPageCatalogoBienResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageCatalogoBienResponse();

                try
                {
                    var filter = _mapper.Map<CatalogoBienFilter>(request.CatalogoBienFilterDto);
                    var reponsePagination = await _catalogoBienAPI.FindPageAsync(filter);

                    if (!reponsePagination.Success)
                    {
                        response.Messages = reponsePagination.Messages;
                        response.Success = false;
                        return response;

                    }

                    foreach (var item in reponsePagination.Data.Items)
                    {
                        item.Saldo = await _detalleRepository.CountByCatalogoBienSaldo(item.CatalogoBienId);
                    }

                    response.Data = _mapper.Map<Pagination<CatalogoBienDto>>(reponsePagination.Data);

                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE));
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
