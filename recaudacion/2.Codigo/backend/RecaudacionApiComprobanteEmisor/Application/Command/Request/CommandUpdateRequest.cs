
using MediatR;
using RecaudacionApiComprobanteEmisor.Application.Command.Dtos;
using RecaudacionApiComprobanteEmisor.Application.Command.Response;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Request
{
    public class CommandUpdate : IRequest<StatusUpdateResponse>
    {
        public int Id { get; set; }
        public ComprobanteEmisorFormDto FormDto { get; set; }
    }
}
