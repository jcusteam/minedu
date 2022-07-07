using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiGuiaSalidaBien.Application.Query.Dtos;
using RecaudacionApiGuiaSalidaBien.DataAccess;
using RecaudacionApiGuiaSalidaBien.Domain;
using MediatR;
using RecaudacionApiGuiaSalidaBien.Clients;
using RecaudacionUtils;

namespace RecaudacionApiGuiaSalidaBien.Application.Query
{
    public class FindByIdGuiaSalidaBienHandler
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
            private readonly IGuiaSalidaBienRepository _repository;
            private readonly IGuiaSalidaBienDetalleRepository _detalleRepository;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly IMapper _mapper;
            public Handler(IGuiaSalidaBienRepository repository,
                IGuiaSalidaBienDetalleRepository detalleRepository,
                ICatalogoBienAPI catalogoBienAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _catalogoBienAPI = catalogoBienAPI;
                _mapper = mapper;
            }
            public async Task<StatusFindResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusFindResponse();

                try
                {
                    var guiaSalidaBien = await _repository.FindById(request.Id);

                    if (guiaSalidaBien == null)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, Message.INFO_NOT_EXISTS_DATA));
                        response.Success = false;
                    }
                    else
                    {
                        var detalles = await _detalleRepository.FindAll(guiaSalidaBien.GuiaSalidaBienId);

                        foreach (var item in detalles)
                        {
                            var catalogoBienResponse = await _catalogoBienAPI.FindByIdAsync(item.CatalogoBienId);

                            if (catalogoBienResponse.Success)
                            {
                                item.CatalogoBien = catalogoBienResponse.Data;
                            }
                        }

                        var guiaSalidaBienDto = _mapper.Map<GuiaSalidaBien, GuiaSalidaBienDto>(guiaSalidaBien);
                        guiaSalidaBienDto.GuiaSalidaBienDetalle = _mapper.Map<List<GuiaSalidaBienDetalleDto>>(detalles); ;
                        response.Data = guiaSalidaBienDto;
                        response.Success = true;
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
