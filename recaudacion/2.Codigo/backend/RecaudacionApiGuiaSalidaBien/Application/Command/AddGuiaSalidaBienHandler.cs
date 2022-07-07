using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Application.Command.Dtos;
using RecaudacionApiGuiaSalidaBien.DataAccess;
using RecaudacionApiGuiaSalidaBien.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionUtils;
using RecaudacionApiGuiaSalidaBien.Clients;
using RecaudacionApiGuiaSalidaBien.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiGuiaSalidaBien.Application.Command
{
    public class AddGuiaSalidaBienHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public GuiaSalidaBienFormDto FormDto { get; set; }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly IIngresoPecosaAPI _ingresoPecosaAPI;
            public DetallesValidator(ICatalogoBienAPI catalogoBienAPI,
                IIngresoPecosaAPI ingresoPecosaAPI)
            {
                _catalogoBienAPI = catalogoBienAPI;
                _ingresoPecosaAPI = ingresoPecosaAPI;

                RuleFor(x => x.FormDto.GuiaSalidaBienDetalle)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("Detalle de Guia Salida Bien no puede ser nulo")
                    .Custom((x, context) =>
                    {
                        if (x.Count == 0)
                        {
                            context.AddFailure($"Detalle de Guia Salida Bien es requerido");
                        }
                    })
                    .ForEach(detalles =>
                    {
                        detalles.ChildRules(detalle =>
                        {
                            detalle.RuleFor(x => x.CatalogoBienId)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Id Catalogo Bien es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 1)
                                    {
                                        context.AddFailure($"Detalle: Id Catalogo Bien no debe ser {x}");
                                    }
                                })
                                .MustAsync(async (id, cancellation) =>
                                {
                                    var response = await _catalogoBienAPI.FindByIdAsync(id);
                                    bool exists = response.Success;
                                    return exists;
                                }).WithMessage("Detalle: Id Catalogo Bien no existe");

                            detalle.RuleFor(x => x.IngresoPecosaDetalleId)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Id Ingreso Pecosa detalle es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 1)
                                    {
                                        context.AddFailure($"Detalle: Id Ingreso Pecosa detalle no debe ser {x}");
                                    }
                                })
                                .MustAsync(async (id, cancellation) =>
                                {
                                    var response = await _ingresoPecosaAPI.FindDetalleByIdAsync(id);
                                    bool exists = response.Success;
                                    return exists;
                                }).WithMessage("Detalle: Id Ingreso Pecosa detalle no existe");

                            detalle.RuleFor(x => x.SerieFormato)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detlle: Serie Formato es requerido")
                                .MaximumLength(GuiaSalidaBienDetalleConsts.SerieFormatoMaxLength)
                                .WithMessage($"La longitud del Serie Formato debe tener {GuiaSalidaBienDetalleConsts.SerieFormatoMaxLength} caracteres o menos")
                                .Custom((x, context) =>
                                {
                                    if (Regex.Replace(x, @"[a-zA-Z0-9]", string.Empty).Length > 0)
                                    {
                                        context.AddFailure("Detalle: Serie Formato contiene caracter no válido.");
                                    }
                                });

                            detalle.RuleFor(x => x.Cantidad)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Cantidad es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 1)
                                    {
                                        context.AddFailure($"Detalle: Cantidad no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.SerieDel)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Serie Del es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Serie Del no debe ser {x}");
                                    }
                                });

                            detalle.RuleFor(x => x.SerieAl)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty().WithMessage("Detalle: Serie Al es requerido")
                                .Custom((x, context) =>
                                {
                                    if (x < 0)
                                    {
                                        context.AddFailure($"Detalle: Serie Al no debe ser {x}");
                                    }
                                });
                        });
                    });
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;

            public CommandValidator(
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI
                )
            {
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;

                RuleFor(x => x.FormDto.UnidadEjecutoraId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Unidad Ejecutora es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != Definition.ID_UE_024)
                        {
                            context.AddFailure($"Unidad Ejecutora permitida {Definition.CODIGO_UE_024}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                    {
                        var response = await _unidadEjecutoraAPI.FindByIdAsync(id);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Id Unidad Ejecutora no existe");

                RuleFor(x => x.FormDto.TipoDocumentoId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Id Tipo Documento es requerido")
                    .Custom((x, context) =>
                    {
                        if (x != Definition.TIPO_DOCUMENTO_GUIA_SALIDA_BIEN)
                        {
                            context.AddFailure($"Id Tipo Documento no debe ser {x}");
                        }
                    })
                    .MustAsync(async (id, cancellation) =>
                    {
                        var response = await _tipoDocumentoAPI.FindByIdAsync(id);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Id Tipo Documento no existe.");

                RuleFor(x => x.FormDto.Estado)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Estado es requerido")
                    .Custom((x, context) =>
                    {
                        if (x < 1)
                        {
                            context.AddFailure($"Estado no debe ser {x}");
                        }
                    })
                    .MustAsync(async (estado, cancellation) =>
                    {
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_GUIA_SALIDA_BIEN, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe");

                RuleFor(x => x.FormDto.Justificacion)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Justificación es requerido")
                    .MaximumLength(GuiaSalidaBienConsts.JustificacionMaxLength)
                    .WithMessage($"La longitud de la Justificación debe tener {GuiaSalidaBienConsts.JustificacionMaxLength} caracteres o menos")
                    .Custom((x, context) =>
                    {
                        if (Regex.Replace(x, @"[A-Za-zÀ-ÿñÑ0-9#/\n(){}¿?¡!""°:;.,_ -]", string.Empty).Length > 0)
                        {
                            context.AddFailure("La Justificación contiene caracter no válido.");
                        }
                    });

                RuleFor(x => x.FormDto.UsuarioCreador)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(GuiaSalidaBienConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {GuiaSalidaBienConsts.UsuarioCreadorMaxLength} caracteres o menos");

            }
        }
        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IGuiaSalidaBienRepository _repository;
            private readonly IGuiaSalidaBienDetalleRepository _detalleRepository;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly IIngresoPecosaAPI _ingresoPecosaAPI;
            private readonly IMapper _mapper;

            public Handler(IGuiaSalidaBienRepository repository,
                IGuiaSalidaBienDetalleRepository detalleRepository,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                ICatalogoBienAPI catalogoBienAPI,
                IIngresoPecosaAPI ingresoPecosaAPI,
                IMapper mapper)
            {
                _repository = repository;
                _detalleRepository = detalleRepository;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
                _catalogoBienAPI = catalogoBienAPI;
                _ingresoPecosaAPI = ingresoPecosaAPI;
                _mapper = mapper;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (!result.IsValid)
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
                        response.Success = false;
                        return response;
                    }

                    DetallesValidator detalleValidations = new DetallesValidator(_catalogoBienAPI, _ingresoPecosaAPI);
                    var detalleResult = await validations.ValidateAsync(request);
                    foreach (var item in detalleResult.Errors)
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        response.Success = false;
                        return response;
                    }

                    var guiaSalidaBien = _mapper.Map<GuiaSalidaBienFormDto, GuiaSalidaBien>(request.FormDto);
                    guiaSalidaBien.TipoDocumentoId = Definition.TIPO_DOCUMENTO_GUIA_SALIDA_BIEN;
                    var numero = await _repository.FindCorrelativo(guiaSalidaBien.UnidadEjecutoraId, guiaSalidaBien.TipoDocumentoId);
                    guiaSalidaBien.Numero = numero;

                    guiaSalidaBien.FechaRegistro = DateTime.Now;
                    guiaSalidaBien.Estado = Definition.GUIA_SALIDA_BIEN_ESTADO_EMITIDO;
                    guiaSalidaBien.FechaCreacion = DateTime.Now;

                    guiaSalidaBien = await _repository.Add(guiaSalidaBien);

                    if (guiaSalidaBien != null)
                    {
                        foreach (var item in request.FormDto.GuiaSalidaBienDetalle)
                        {
                            var detalle = _mapper.Map<GuiaSalidaBienDetalleFormDto, GuiaSalidaBienDetalle>(item);
                            detalle.GuiaSalidaBienId = guiaSalidaBien.GuiaSalidaBienId;
                            detalle.FechaCreacion = DateTime.Now;
                            detalle.Estado = "1";
                            await _detalleRepository.Add(detalle);
                        }

                        response.Data = _mapper.Map<GuiaSalidaBien, GuiaSalidaBienFormDto>(guiaSalidaBien);
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
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
