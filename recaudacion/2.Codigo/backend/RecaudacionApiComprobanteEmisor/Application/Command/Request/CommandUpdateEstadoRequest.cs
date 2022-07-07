
using MediatR;
using RecaudacionApiComprobanteEmisor.Application.Command.Dtos;
using RecaudacionApiComprobanteEmisor.Application.Command.Response;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Request
{
    public class CommandUpdateEstado : IRequest<StatusUpdateEstadoResponse>
    {
        public int Id { get; set; }
        public ComprobanteEmisorEstadoFormDto FormDto { get; set; }
    }
}
