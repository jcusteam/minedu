using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiGuiaSalidaBien.Application.Query.Dtos;
using RecaudacionApiGuiaSalidaBien.DataAccess;
using MediatR;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionApiGuiaSalidaBien.Clients;
using RecaudacionUtils;

namespace RecaudacionApiGuiaSalidaBien.Application.Query
{
    public class PageGuiaSalidaBienHandler
    {
        public class StatusPageResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusPageResponse>
        {
            public GuiaSalidaBienFilterDto GuiaSalidaBienFilterDto { get; set; }
        }

        public class Handler : IRequestHandler<Query, StatusPageResponse>
        {
            private readonly IGuiaSalidaBienRepository _repository;
            private readonly IMapper _mapper;
            private readonly IEstadoAPI _estadoAPI;
            public Handler(IGuiaSalidaBienRepository repository,
                IEstadoAPI estadoAPI,
                IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
                _estadoAPI = estadoAPI;
            }

            public async Task<StatusPageResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusPageResponse();

                try
                {
                    var filter = _mapper.Map<GuiaSalidaBienFilter>(request.GuiaSalidaBienFilterDto);
                    var pagination = await _repository.FindPage(filter);
                    var total = await _repository.Count(filter);

                    foreach (var item in pagination.Items)
                    {
                        var estadoResponse = await _estadoAPI.FindByTipoDocAndNumeroAsync(item.TipoDocumentoId, item.Estado);
                        if (estadoResponse.Success)
                        {
                            item.EstadoNombre = estadoResponse.Data.Nombre;
                        }
                    }

                    response.Data = _mapper.Map<Pagination<GuiaSalidaBienDto>>(pagination);

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
