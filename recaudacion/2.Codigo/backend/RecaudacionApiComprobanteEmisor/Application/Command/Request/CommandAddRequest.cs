
using MediatR;
using RecaudacionApiComprobanteEmisor.Application.Command.Dtos;
using RecaudacionApiComprobanteEmisor.Application.Command.Response;

namespace RecaudacionApiComprobanteEmisor.Application.Command.Request
{
    public class CommandAdd : IRequest<StatusAddResponse>
    {
        public ComprobanteEmisorFormDto FormDto { get; set; }
    }
}
