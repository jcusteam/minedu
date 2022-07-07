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
    public class FindAllCatalogoBienIngresoPecosaDetalleHandler
    {

        public class StatusFindAllCatalogoBienResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllCatalogoBienResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllCatalogoBienResponse>
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
            public async Task<StatusFindAllCatalogoBienResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllCatalogoBienResponse();
                try
                {

                    var catalogoBienReponse = await _catalogoBienAPI.FindAllAsync();

                    if (catalogoBienReponse.Success)
                    {

                        if (catalogoBienReponse.Data.Count == 0)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA_ITEMS));
                            response.Success = false;
                            return response;
                        }

                        foreach (var item in catalogoBienReponse.Data)
                        {
                            item.Saldo = await _detalleRepository.CountByCatalogoBienSaldo(item.CatalogoBienId);
                        }
                        response.Data = catalogoBienReponse.Data;
                        response.Success = true;
                    }
                    else
                    {
                        response.Messages = catalogoBienReponse.Messages;
                        response.Success = false;
                    }

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
