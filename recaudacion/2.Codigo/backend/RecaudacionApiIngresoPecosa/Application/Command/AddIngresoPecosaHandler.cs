using System;
using System.Threading;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Application.Command.Dtos;
using RecaudacionApiIngresoPecosa.DataAccess;
using RecaudacionApiIngresoPecosa.Domain;
using FluentValidation;
using MediatR;
using AutoMapper;
using RecaudacionApiIngresoPecosa.Clients;
using RecaudacionUtils;
using RecaudacionApiIngresoPecosa.Helpers;
using System.Text.RegularExpressions;

namespace RecaudacionApiIngresoPecosa.Application.Command
{
    public class AddIngresoPecosaHandler
    {
        public class StatusAddResponse : StatusResponse<object>
        {
        }
        public class Command : IRequest<StatusAddResponse>
        {
            public IngresoPecosaFormDto FormDto { get; set; }
        }

        public class DetalleValidator : AbstractValidator<IngresoPecosaDetalleFormDto>
        {
            public DetalleValidator()
            {


            }
        }

        public class DetallesValidator : AbstractValidator<Command>
        {
            public DetallesValidator()
            {

                RuleFor(x => x.FormDto.IngresoPecosaDetalle)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Detalle de Ingreso pecosa no puede ser nulo")
                .Custom((x, context) =>
                {
                    if (x.Count == 0)
                    {
                        context.AddFailure($"Detalle de Ingreso pecosa es requerido");
                    }
                })
                .ForEach(detalles =>
                {
                    detalles.ChildRules(detalle =>
                    {
                        detalle.RuleFor(x => x.CodigoItem)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage("Detalle: Código item es requerido")
                            .MaximumLength(IngresoPecosaDetalleConsts.CodigoItemMaxLength)
                            .WithMessage($"Detalle: La longitud del Código item debe tener {IngresoPecosaDetalleConsts.CodigoItemMaxLength} caracteres o menos");

                        detalle.RuleFor(x => x.NombreItem)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage("Detalle: Nombre item es requerido")
                            .MaximumLength(IngresoPecosaDetalleConsts.NombreItemMaxLength)
                            .WithMessage($"La longitud del Nombre item debe tener {IngresoPecosaDetalleConsts.NombreItemMaxLength} caracteres o menos");

                        detalle.RuleFor(x => x.NombreMarca)
                           .Cascade(CascadeMode.Stop)
                           .NotEmpty().WithMessage("Detalle: Nombre Marca es requerido")
                           .MaximumLength(IngresoPecosaDetalleConsts.NombreMarcaMaxLength)
                           .WithMessage($"Detalle: La longitud del Nombre Marca debe tener {IngresoPecosaDetalleConsts.NombreMarcaMaxLength} caracteres o menos");


                        detalle.RuleFor(x => x.UnidadMedida)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage("Detalle: Unidad medida es requerido")
                            .MaximumLength(IngresoPecosaDetalleConsts.UnidadMedidaMaxLength)
                            .WithMessage($"La longitud de la Unidad medida debe tener {IngresoPecosaDetalleConsts.UnidadMedidaMaxLength} caracteres o menos");

                        detalle.RuleFor(x => x.Cantidad)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage("Detalle: Cantidad es requerido")
                            .Custom((x, context) =>
                            {
                                if (x <= 0)
                                {
                                    context.AddFailure($"Detalle: Cantidad no debe ser {x}");
                                }
                            });

                        detalle.RuleFor(x => x.SerieFormato)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage("Detalle: Serie formato es requerido")
                            .MaximumLength(IngresoPecosaDetalleConsts.SerieFormatoMaxLength)
                            .WithMessage($"La longitud de la Serie formato debe tener {IngresoPecosaDetalleConsts.SerieFormatoMaxLength} caracteres o menos");

                        detalle.RuleFor(x => x.SerieAl)
                           .Cascade(CascadeMode.Stop)
                           .NotEmpty().WithMessage("Detalle: Serie Al es requerido")
                           .Custom((x, context) =>
                           {
                               if (x <= 0)
                               {
                                   context.AddFailure($"Detalle: Serie Al no debe ser {x}");
                               }
                           });

                        detalle.RuleFor(x => x.SerieDel)
                           .Cascade(CascadeMode.Stop)
                           .NotEmpty().WithMessage("Detalle: Serie Del es requerido")
                           .Custom((x, context) =>
                           {
                               if (x <= 0)
                               {
                                   context.AddFailure($"Detalle: Serie Del no debe ser {x}");
                               }
                           });

                        detalle.RuleFor(x => x.PrecioUnitario)
                           .Cascade(CascadeMode.Stop)
                           .Custom((x, context) =>
                           {
                               if (x < 0)
                               {
                                   context.AddFailure($"Detalle: Precio Unitario no debe ser {x}");
                               }
                           });

                        detalle.RuleFor(x => x.ValorTotal)
                           .Cascade(CascadeMode.Stop)
                           .Custom((x, context) =>
                           {
                               if (x < 0)
                               {
                                   context.AddFailure($"Detalle: Valor Total no debe ser {x}");
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
                ITipoDocumentoAPI tipoDocumentoAPI)
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
                    }).WithMessage("Id Unidad Ejecutora no existe.");

                RuleFor(x => x.FormDto.TipoDocumentoId)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Id Tipo Documento es requerido")
                   .Custom((x, context) =>
                   {
                       if (x != Definition.TIPO_DOCUMENTO_INGRESO_PECOSA)
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
                        var response = await _estadoAPI.FindByTipoDocAndNumeroAsync(Definition.TIPO_DOCUMENTO_INGRESO_PECOSA, estado);
                        bool exists = response.Success;
                        return exists;
                    }).WithMessage("Número de estado no existe.");

                RuleFor(x => x.FormDto.TipoBien)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Tipo Bien es requerido")
                    .MaximumLength(IngresoPecosaConsts.TipoBienMaxLength)
                    .WithMessage($"La longitud del Tipo bien debe tener {IngresoPecosaConsts.TipoBienMaxLength} caracteres o menos");

                RuleFor(x => x.FormDto.AnioPecosa)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Año es requerido")
                    .Custom((x, context) =>
                     {
                         var anio = x;
                         var year = DateTime.Now.Year;
                         if (anio < IngresoPecosaConsts.AnioMin || anio > year)
                         {
                             context.AddFailure($"El Año debe ser entre los valores del {IngresoPecosaConsts.AnioMin} al {year}");
                         }
                     });

                RuleFor(x => x.FormDto.NombreAlmacen)
                   .Cascade(CascadeMode.Stop)
                   .MaximumLength(IngresoPecosaConsts.NombreAlmacenMaxLength)
                   .WithMessage($"La longitud del Nombre almacén debe tener {IngresoPecosaConsts.NombreAlmacenMaxLength} caracteres o menos");

                RuleFor(x => x.FormDto.MotivoPedido)
                  .Cascade(CascadeMode.Stop)
                  .MaximumLength(IngresoPecosaConsts.MotivoPedidoMaxLength)
                  .WithMessage($"La longitud del Motivo Pedido debe tener {IngresoPecosaConsts.MotivoPedidoMaxLength} caracteres o menos");

                RuleFor(x => x.FormDto.UsuarioCreador)
                    .NotEmpty().WithMessage("Usuario Creador es requerido")
                    .MaximumLength(IngresoPecosaConsts.UsuarioCreadorMaxLength)
                    .WithMessage($"La longitud del Usuario Creador debe tener {IngresoPecosaConsts.UsuarioCreadorMaxLength} caracteres o menos");
            }
        }

        public class Handler : IRequestHandler<Command, StatusAddResponse>
        {
            private readonly IIngresoPecosaRepository _repository;
            private readonly IIngresoPecosaDetalleRepository _detalleRepository;
            private readonly ICatalogoBienAPI _catalogoBienAPI;
            private readonly IEstadoAPI _estadoAPI;
            private readonly IUnidadEjecutoraAPI _unidadEjecutoraAPI;
            private readonly ITipoDocumentoAPI _tipoDocumentoAPI;
            private readonly IPedidoPecosaAPI _pedidoPecosaAPI;
            private readonly IMapper _mapper;

            public Handler(IIngresoPecosaRepository repository,
                IIngresoPecosaDetalleRepository detalleRepository,
                ICatalogoBienAPI catalogoBienAPI,
                IEstadoAPI estadoAPI,
                IUnidadEjecutoraAPI unidadEjecutoraAPI,
                ITipoDocumentoAPI tipoDocumentoAPI,
                IPedidoPecosaAPI pedidoPecosaAPI,
                IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
                _detalleRepository = detalleRepository;
                _catalogoBienAPI = catalogoBienAPI;
                _estadoAPI = estadoAPI;
                _unidadEjecutoraAPI = unidadEjecutoraAPI;
                _pedidoPecosaAPI = pedidoPecosaAPI;
                _tipoDocumentoAPI = tipoDocumentoAPI;
            }

            public async Task<StatusAddResponse> Handle(Command request, CancellationToken cancellationToken)
            {

                var response = new StatusAddResponse();
                try
                {
                    CommandValidator validations = new CommandValidator(_estadoAPI, _unidadEjecutoraAPI, _tipoDocumentoAPI);
                    var result = await validations.ValidateAsync(request);

                    if (result.IsValid)
                    {
                        DetalleValidator detalleValidations = new DetalleValidator();
                        var detalleResult = validations.Validate(request);

                        foreach (var item in detalleResult.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                            response.Success = false;
                            return response;
                        }

                        var ingresoPecosa = _mapper.Map<IngresoPecosaFormDto, IngresoPecosa>(request.FormDto);
                        ingresoPecosa.FechaRegistro = DateTime.Now;
                        ingresoPecosa.FechaCreacion = DateTime.Now;
                        ingresoPecosa.TipoDocumentoId = Definition.TIPO_DOCUMENTO_INGRESO_PECOSA;

                        var status = await _repository.VerifyExists(Definition.INSERT, ingresoPecosa);
                        if (!status)
                        {
                            var unidadEjecutoraResponse = await _unidadEjecutoraAPI.FindByIdAsync(ingresoPecosa.UnidadEjecutoraId);

                            if (unidadEjecutoraResponse.Success)
                            {
                                var unidadEjecutora = unidadEjecutoraResponse.Data;
                                var pedidoPecosaResponse = await _pedidoPecosaAPI.FindByEjecutoraAsync(unidadEjecutora.Codigo, ingresoPecosa.AnioPecosa, ingresoPecosa.NumeroPecosa);

                                if (pedidoPecosaResponse.Success)
                                {
                                    ingresoPecosa.TipoBien = pedidoPecosaResponse.Data.TipoBien;
                                    ingresoPecosa.FechaPecosa = pedidoPecosaResponse.Data.FechaPecosa;
                                    ingresoPecosa.NombreAlmacen = pedidoPecosaResponse.Data.NombreAlmacen;
                                    ingresoPecosa.MotivoPedido = pedidoPecosaResponse.Data.MotivoPedido;
                                }
                                else
                                {
                                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "No se podido obtener la pecosa con el número " + ingresoPecosa.NumeroPecosa));
                                    response.Success = false;
                                    return response;
                                }
                            }
                            else
                            {
                                response.Messages = unidadEjecutoraResponse.Messages;
                                response.Success = false;
                                return response;
                            }


                            ingresoPecosa = await _repository.Add(ingresoPecosa);

                            if (ingresoPecosa != null)
                            {
                                foreach (var item in request.FormDto.IngresoPecosaDetalle)
                                {
                                    var detalle = _mapper.Map<IngresoPecosaDetalleFormDto, IngresoPecosaDetalle>(item);
                                    var catalogoBienId = 0;

                                    var catalogoBienResponse = await _catalogoBienAPI.FindByCodigoAsync(item.CodigoItem);
                                    if (catalogoBienResponse.Success)
                                    {
                                        catalogoBienId = catalogoBienResponse.Data.CatalogoBienId;
                                    }
                                    else
                                    {
                                        var catalogoBien = new CatalogoBien();
                                        catalogoBien.Codigo = item.CodigoItem;
                                        catalogoBien.ClasificadorIngresoId = Definition.ID_CLASIFICADOR_INGRESO_OTRO_PRODUCTO;
                                        catalogoBien.UnidadMedidaId = Definition.ID_UNIDAD_MEDIDA_UNIDAD;
                                        catalogoBien.Codigo = item.CodigoItem;
                                        catalogoBien.Descripcion = item.NombreItem;
                                        catalogoBien.StockMaximo = 0;
                                        catalogoBien.StockMinimo = 0;
                                        catalogoBien.PuntoReorden = 0;
                                        catalogoBien.Estado = true;
                                        catalogoBien.UsuarioCreador = ingresoPecosa.UsuarioCreador;

                                        var catalogoBienAddResponse = await _catalogoBienAPI.AddAsync(catalogoBien);
                                        if (catalogoBienAddResponse.Success)
                                        {
                                            catalogoBienId = catalogoBienAddResponse.Data.CatalogoBienId;
                                        }
                                    }

                                    detalle.IngresoPecosaId = ingresoPecosa.IngresoPecosaId;
                                    detalle.CatalogoBienId = catalogoBienId;
                                    detalle.Estado = "1";
                                    detalle.FechaCreacion = DateTime.Now;
                                    detalle.UsuarioCreador = ingresoPecosa.UsuarioCreador;
                                    await _detalleRepository.Add(detalle);
                                }

                                response.Data = _mapper.Map<IngresoPecosa, IngresoPecosaFormDto>(ingresoPecosa);
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_SUCCESS, Message.SUCCESS_INSERT));
                                response.Success = true;
                            }
                            else
                            {
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_INSERT_DB));
                                response.Success = false;
                            }
                        }
                        else
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, "Ya se encuentra registrado la pecosa con el número " + ingresoPecosa.NumeroPecosa + "."));
                            response.Success = false;
                        }
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_WARNING, item.ErrorMessage));
                        }
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
