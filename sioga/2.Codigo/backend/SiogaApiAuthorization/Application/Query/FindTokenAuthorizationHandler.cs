using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SiogaApiAuthorization.Service.Contracts;
using SiogaApiAuthorization.Application.Query.Dtos;
using SiogaUtils;
using FluentValidation;
using SiogaApiAuthorization.Clients;
using AutoMapper;
using SiogaApiAuthorization.Domain;

namespace SiogaApiAuthorization.Application.Query
{
    public class FindTokenAuthorizationHandler
    {
        public class StatusTokenResponse : StatusApiResponse<object>
        {
        }
        public class Query : IRequest<StatusTokenResponse>
        {
            public AuthDto AuthDto { get; set; }
            public string HeaderAuth { get; set; }
        }

        public class CommandValidator : AbstractValidator<Query>
        {
            public CommandValidator()
            {
                RuleFor(x => x.HeaderAuth)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Authorization Bearer es requerido");

                RuleFor(x => x.AuthDto.CodigoModulo)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Codigo Modulo es requerido");
            }
        }

        public class Handler : IRequestHandler<Query, StatusTokenResponse>
        {
            private readonly IAutorizationService _authorizationService;
            private readonly IUsuarioInstitucionAPI _usuarioInstitucionAPI;
            private readonly AppSettings _appSettings;
            private readonly IMapper _mapper;

            public Handler(IAutorizationService authorizationService,
                IUsuarioInstitucionAPI usuarioInstitucionAPI,
                AppSettings appSettings,
                IMapper mapper)
            {
                _authorizationService = authorizationService;
                _usuarioInstitucionAPI = usuarioInstitucionAPI;
                _appSettings = appSettings;
                _mapper = mapper;
            }

            public async Task<StatusTokenResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var response = new StatusTokenResponse();
                try
                {

                    CommandValidator validations = new CommandValidator();
                    var result = validations.Validate(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    var usuarioResponse = await _authorizationService.GetUsuarioToken(request.AuthDto.CodigoModulo, request.HeaderAuth);

                    if (!usuarioResponse.Success)
                    {
                        response.Messages = usuarioResponse.Messages;
                        response.Success = false;
                        return response;
                    }

                    response.Data = usuarioResponse.Data;

                    if (request.AuthDto.CodigoModulo == _appSettings.Modulo.Subvencion.Codigo)
                    {
                        var headerAuth = $"Bearer {usuarioResponse.Data.Token}";
                        var usuarioInstitcionResponse = await _usuarioInstitucionAPI.FindByNroDocAsync(usuarioResponse.Data.Usuario.NumeroDocumento, headerAuth);
                        if (usuarioInstitcionResponse.Success)
                        {
                            var usuaritoToken = _mapper.Map<UsuarioToken, UsuarioInstitucionTokenDto>(usuarioResponse.Data);
                            usuaritoToken.Institucion = usuarioInstitcionResponse.Data.Institucion;
                            response.Data = usuaritoToken;

                        }
                    }


                    response.Messages = usuarioResponse.Messages;

                }
                catch (System.Exception e)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, e.Message));
                    response.Success = false;
                }

                return response;
            }
        }
    }
}
