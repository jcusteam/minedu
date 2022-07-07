using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using RecaudacionApiGuiaSalidaBien.Application.Query.Dtos;
using RecaudacionApiGuiaSalidaBien.DataAccess;
using MediatR;
using RecaudacionUtils;

namespace RecaudacionApiGuiaSalidaBien.Application.Query
{
    public class FindAllGuiaSalidaBienHandler
    {
        public class StatusFindAllResponse : StatusResponse<object>
        {
        }
        public class Query : IRequest<StatusFindAllResponse>
        {
        }

        public class Handler : IRequestHandler<Query, StatusFindAllResponse>
        {
            private readonly IGuiaSalidaBienRepository _repository;
            private readonly IMapper _mapper;
            public Handler(IGuiaSalidaBienRepository repository, IMapper mapper)
            {
                _mapper = mapper;
                _repository = repository;
            }

            public async Task<StatusFindAllResponse> Handle(Query request, CancellationToken cancellationToken)
            {

                var response = new StatusFindAllResponse();

                try
                {
                    var items = await _repository.FindAll();
                    response.Data = _mapper.Map<List<GuiaSalidaBienDto>>(items);
                    response.Success = true;
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
