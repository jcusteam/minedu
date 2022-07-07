import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { DatePipe } from "@angular/common";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, } from "@angular/material/core";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from "@angular/material-moment-adapter";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, } from "@angular/material/dialog";
import { MatTableDataSource } from "@angular/material/table";

import { TipoDocEnum, ValidaDepositoEnum, TipoDocIdentidadEnum, TipoAdquisicionEnum, TipoCaptacionEnum, TipoMonedaEnum, TipoOperacionEnum, TipoIGVEnum, IGVEnum, TipoComprobanteEnum, FuenteOrigenEnum, FuenteValidaEnum, TipoPrecioVentaEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { CatalogoBien } from "src/app/core/models/catalogobien";
import { Cliente } from "src/app/core/models/cliente";
import { ComprobantePagoDetalle, ComprobantePago, ComprobantePagoFuente } from "src/app/core/models/comprobantepago";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { DepositoBancoDetalle } from "src/app/core/models/depositobanco";
import { IngresoPecosaDetalle } from "src/app/core/models/ingresopecosa";
import { Tarifario } from "src/app/core/models/tarifario";
import { TipoCaptacion } from "src/app/core/models/tipocaptacion";
import { TipoComprobantePago } from "src/app/core/models/tipocomprobantepago";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { AuthService } from "src/app/core/services/auth.service";
import { CatalogoBienesService } from "src/app/core/services/catalogo-bienes.service";
import { ClienteService } from "src/app/core/services/cliente.service";
import { ComprobantePagoService } from "src/app/core/services/comprobante-pago.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { DepositoBancoService } from "src/app/core/services/deposito-banco.service";
import { MessageService } from "src/app/core/services/message.service";
import { PideService } from "src/app/core/services/pide.service";
import { TarifarioService } from "src/app/core/services/tarifario.service";
import { TipoCaptacionService } from "src/app/core/services/tipo-captacion.service";
import { TipoComprobantePagoService } from "src/app/core/services/tipo-comprobante-pago.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { TransversalService } from "src/app/core/services/transversal.service";
import { TYPE_MESSAGE, MESSAGES } from "src/app/core/utils/messages";
import { Tools } from "src/app/core/utils/tools";
import { VALIDATION } from "src/app/core/utils/validation";
import { ListSaldoIngresoNotaDebitoComponent } from "../list-saldo-ingreso/list-saldo-ingreso.component";
import { NewNotaDebitoClienteComponent } from "../new-nota-debito-cliente/new-nota-debito-cliente.component";

const DATE_MODE_FORMATS = {
  parse: {
    dateInput: "DD/MM/YYYY",
  },
  display: {
    dateInput: "DD/MM/YYYY",
    monthYearLabel: "MMM YYYY",
    dateA11yLabel: "LL",
    monthYearA11yLabel: "MMMM YYYY",
  },
};

@Component({
  selector: "app-new-nota-debito",
  templateUrl: "./new-nota-debito.component.html",
  styleUrls: ["./new-nota-debito.component.scss"],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
  ],
})
export class NewNotaDebitoComponent implements OnInit {
  // Enums
  tipoDocEnum = TipoDocEnum;
  validaDepositoEnum = ValidaDepositoEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoAdquisicionEnum = TipoAdquisicionEnum;
  tipoCaptacionEnum = TipoCaptacionEnum;
  tipoMonedaEnum = TipoMonedaEnum;
  tipoOperacionEnum = TipoOperacionEnum;
  tipoIGVEnum = TipoIGVEnum;
  IGVEnum = IGVEnum;
  tipoComprobanteEnum = TipoComprobanteEnum;
  fuenteOrigenEnum = FuenteOrigenEnum;
  fuenteValidaEnum = FuenteValidaEnum;
  tipoPrecioVentaEnum = TipoPrecioVentaEnum;

  // Tarifarios
  tarifarios: Tarifario[] = [];
  filteredTarifarios;

  // Catalogo de bienes
  catalogoBienes: CatalogoBien[] = [];
  filteredCatalogoBienes;

  // Cuentas Corrientes
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredCuentasCorrientes;

  // Tipo Doc Identidad
  tipoDocIdentidades: TipoDocumentoIdentidad[] = [];
  tipoDocIdentidadesEncargado: TipoDocumentoIdentidad[] = [];
  tipoFuentes: TipoComprobantePago[] = [];

  hiddenDeposito = true;
  hiddenCheque = true;
  hiddenCatalogo = true;
  hiddenTarifario = false;
  hiddenDocFuente = true;
  hiddenValidaFuente = true;

  // Combobox
  tipoOperaciones: Combobox[] = [];
  tipoMonedas: Combobox[] = [];
  tipoCaptaciones: TipoCaptacion[] = [];
  tipoAdquisiciones: Combobox[] = [];
  tipoIgvs: Combobox[] = [];
  tipoCondicionPagos: Combobox[] = [];
  tipoNotaCreditos: Combobox[] = [];
  tipoNotaDebitos: Combobox[] = [];
  fuenteOrigenes: Combobox[] = [];

  // Nombre de Columna de la tabla
  displayedColumns: string[] = [
    "nro",
    "descripcion",
    "cantidad",
    "precio",
    "descuento",
    "subTotal",
    "valorVenta",
    "actions",
  ];

  // Listado detalle Comprobante 
  comprobantePagoDetalles: ComprobantePagoDetalle[] = [];

  // Data Source
  dataSource = new MatTableDataSource(this.comprobantePagoDetalles);

  // Fecha
  minDateEmision = new Date();
  minDate = new Date();
  maxDate = new Date();

  // Form
  form: FormGroup;

  // Constructor
  constructor(
    public dialogRef: MatDialogRef<NewNotaDebitoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobantePago,
    public dialog: MatDialog,
    private datePipe: DatePipe,
    private fb: FormBuilder,
    private comprobantePagoService: ComprobantePagoService,
    private clienteService: ClienteService,
    private catalogoBienService: CatalogoBienesService,
    private tarifarioService: TarifarioService,
    private tipoCaptacionService: TipoCaptacionService,
    private tipoComprobantePagoService: TipoComprobantePagoService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private cuentaCorrienteService: CuentaCorrienteService,
    private messageService: MessageService,
    private transversalService: TransversalService,
    private depositoBancoService: DepositoBancoService,
    private pideService: PideService,
    private authService: AuthService
  ) {

    // Formulario 
    this.form = this.fb.group({
      comprobantePagoId: [0],
      clienteId: [0],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      tipoComprobanteId: [data?.tipoComprobanteId],
      tipoDocumentoId: [data?.tipoDocumentoId],
      depositoBancoDetalleId: [null],
      cuentaCorrienteId: [null],
      tipoCaptacionId: [null,
        Validators.compose([
          Validators.required
        ])
      ],
      comprobanteEmisor: [0],
      serie: [null],
      correlativo: [null],
      fechaEmision: [null,
        Validators.compose([
          Validators.required
        ])
      ],
      fechaVencimiento: [null,
        Validators.compose([
          Validators.required
        ])
      ],
      tipoAdquisicion: [data?.tipoAdquisicion,
      Validators.compose([
        Validators.required
      ])
      ],
      codigoTipoOperacion: [data?.codigoTipoOperacion,
      Validators.compose([
        Validators.required
      ])
      ],
      tipoCondicionPago: [null,
        Validators.compose([
          Validators.required
        ])
      ],
      numeroDeposito: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.NUMERODEPOSITO_MAX_LENGTH)
        ])],
      fechaDeposito: [null],
      validarDeposito: [null],
      numeroCheque: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.NUMEROCHEQUE_MAX_LENGTH)
        ])
      ],
      encargadoTipoDocumento: [null],
      encargadoNombre: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.ENCARGADO_NOMBRE_MAX_LENGTH)
        ])
      ],
      encargadoNumeroDocumento: [null,
        Validators.compose([
          Validators.pattern("^[a-zA-Z0-9]$"),
          Validators.maxLength(VALIDATION.COMPROBANTE.ENCARGADO_NUMERO_DOCUMENTO_MAX_LENGTH)
        ])
      ],
      fuenteId: [null],
      fuenteTipoDocumento: [null],
      fuenteSerie: [null],
      fuenteCorrelativo: [null],
      fuenteOrigen: [null],
      fuenteValidar: [null],
      sustento: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.SUSTENTO_MAX_LENGTH)
        ])
      ],
      observacion: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.OBSERVACION_MAX_LENGTH)
        ])
      ],
      nombreArchivo: ["-"],
      tipoCambio: [0],
      pagado: [false],
      estadoSunat: [null],
      codigoTipoMoneda: [this.tipoMonedaEnum.NUEVO_SOL],
      importeBruto: [0],
      valorIGV: [this.IGVEnum.PORCENTAJE],
      igvTotal: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      iscTotal: [0],
      otrTotal: [0],
      otrcTotal: [0],
      importeTotal: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      importeTotalLetra: [null],
      totalOpGravada: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      totalOpInafecta: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      totalOpExonerada: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      totalOpGratuita: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      totalDescuento: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      ordenCompra: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.ORDENCOMPRA_MAX_LENGTH)
        ])
      ],
      guiaRemision: [null,
        Validators.compose([
          Validators.maxLength(VALIDATION.COMPROBANTE.OBSERVACION_MAX_LENGTH)
        ])
      ],
      codigoTipoNota: [null],
      codigoMotivoNota: [null],
      estado: [data?.estado],
      usuarioCreador: [data?.usuarioCreador],
      usuarioModificador: [null],

      // Extra
      tipoDocumentoIdentidadId: [
        null,
        Validators.compose([
          Validators.required
        ]),
      ],
      numeroDocumento: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]$"),
        ]),
      ],
      nombreCliente: [null, Validators.compose([Validators.required])],
      cuentaCorriente: [null],

      cuentaCorrienteValida: [false],
      importeDeposito: [null],
      // Detalle
      catalogoBien: [null],
      tarifario: [null],
      cantidad: [1,
        Validators.compose([
          Validators.pattern("^[0-9]+$"),
          Validators.min(1),
          Validators.maxLength(10)
        ])
      ],
      precioUnitario: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      descuentoItem: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
      codigoTipoIGV: [this.tipoIGVEnum.INAFECTA]
    });
  }

  // Inicio Angular
  ngOnInit() {
    this.minDateEmision = Tools.addDays(new Date(), -2);
    this.onLoadMaestras();
    this.validationDocFuente();
  }

  // Maestras
  onLoadMaestras() {
    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService.getTipoDocumentoIdentidades().subscribe((response) => {
      if (response.success) {
        if (response.success) {
          let tipoDocsFactura = response.data;
          let tipoDocsBoleta = response.data;
          let tipoDocsEncargado = response.data;
          this.tipoDocIdentidadesEncargado = tipoDocsEncargado.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI || x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE);

          switch (this.data.tipoDocumentoId) {
            case this.tipoDocEnum.FACTURA:
              this.tipoDocIdentidades = tipoDocsFactura.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.RUC);
              break;
            case this.tipoDocEnum.BOLETA_VENTA:
              this.tipoDocIdentidades = tipoDocsBoleta.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI || x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE);
              break;
            case this.tipoDocEnum.NOTA_CREDITO:
              this.tipoDocIdentidades = response.data;
              break;
            case this.tipoDocEnum.NOTA_DEBITO:
              this.tipoDocIdentidades = response.data;
              break;
            default:
              break;
          }
        }

      }
    });
    // Cuentas Corrientes
    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId).subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.numeroDenominacion = element.numero + " - " + element.denominacion;
        });
        this.cuentasCorrientes = response.data;
        this.filteredCuentasCorrientes = this.cuentasCorrientes.slice();
      }
    });

    // Tarifarios
    this.tarifarioService.getTarifarios().subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.codigoNombre = element.codigo + ' - ' + element.nombre;
        });
        this.tarifarios = response.data;
        this.filteredTarifarios = this.tarifarios.slice();
      }
    });

    // Catalogos
    this.catalogoBienService.getCatalogoBienes().subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.codigoDescripcion = element.codigo + " - " + element.descripcion;
        });
        this.catalogoBienes = response.data;
        this.filteredCatalogoBienes = this.catalogoBienes.slice();
      }
    });

    // Tipo Captaciones
    this.tipoCaptacionService.getTipoCaptaciones().subscribe((response) => {
      if (response.success) {
        this.tipoCaptaciones = response.data.filter(x => x.tipoCaptacionId != this.tipoCaptacionEnum.VARIOS);
      }
    });

    // tipo operaciones
    this.transversalService.getTipoOperaciones().subscribe((response) => {
      if (response.success) {
        this.tipoOperaciones = response.data;
      }
    });

    // tipo monedas
    this.transversalService.getTipoMonedas().subscribe((response) => {
      if (response.success) {
        this.tipoMonedas = response.data;
      }
    });

    // tipo Adquisicion
    this.transversalService.getTipoAdquisiciones().subscribe((response) => {
      if (response.success) {
        this.tipoAdquisiciones = response.data;
      }
    });

    // tipo IGV
    this.transversalService.getTipoIgvs().subscribe((response) => {
      if (response.success) {
        this.tipoIgvs = response.data;
      }
    });

    // tipo IGV
    this.transversalService.getTipoCondicionPagos().subscribe((response) => {
      if (response.success) {
        this.tipoCondicionPagos = response.data;
      }
    });

    // Tipo Fuentes
    this.tipoComprobantePagoService.getTipoComprobantePagos().subscribe((response) => {
      if (response.success) {
        this.tipoFuentes = response.data.filter(x => x.tipoComprobantePagoId == this.tipoComprobanteEnum.FACTURA || x.tipoComprobantePagoId == this.tipoComprobanteEnum.BOLETA_VENTA)
      }
    });

    // tipo Nota creditos
    this.transversalService.getTipoNotaCreditos().subscribe((response) => {
      if (response.success) {
        this.tipoNotaCreditos = response.data;
      }
    });

    // tipo Nota Débitos
    this.transversalService.getTipoNotaDebitos().subscribe((response) => {
      if (response.success) {
        this.tipoNotaDebitos = response.data;
      }
    });

    // Fuente origenes
    this.transversalService.getFuenteOrigenes().subscribe((response) => {
      if (response.success) {
        this.fuenteOrigenes = response.data;
      }
    });

  }

  // key Up Nro Cliente
  onKeyUpNroDocCliente(event) {
    this.form.patchValue({
      nombreCliente: null,
      clienteId: 0,
    });
  }

  // Buscar cliente
  searchCliente() {

    if (!this.form.get("tipoDocumentoIdentidadId").valid) {
      return;
    }
    if (!this.form.get("numeroDocumento").valid) {
      return;
    }

    const tipoDocumentoIdentidadId = this.form.get("tipoDocumentoIdentidadId").value;

    const numeroDocumento = this.form.get("numeroDocumento").value;
    this.messageService.msgLoad("Consultando Cliente...");
    this.clienteService.getClienteByTipoNroDocumento(tipoDocumentoIdentidadId, numeroDocumento)
      .subscribe(
        (response) => {
          var message = response.messages.join(",");
          this.messageService.msgAutoClose();
          if (response.success) {
            this.form.patchValue({ clienteId: response.data.clienteId, nombreCliente: response.data.nombre })
          }
          else {
            let type = response.messageType;
            if (type == TYPE_MESSAGE.WARNING) {
              this.messageService.msgWarning(message, () => { });
            }
            else if (type == TYPE_MESSAGE.INFO) {
              this.messageService.msgWarning(message, () => { });
            }
            else {
              this.messageService.msgError(message, () => { });
            }
          }
        },
        (error) => {
          this.handleError(error);
        }
      );
  }

  // Seleción de tipo doc identidad
  selectedChangeTipoDocIdentidad(id: number) {
    this.form.patchValue({
      numeroDocumento: null,
      nombreCliente: null,
      clienteId: 0
    });

    this.valiationTipoDocIdentidad(id);
  }

  // validación de tipo doc identidad
  valiationTipoDocIdentidad(id: number) {
    if (id == this.tipoDocIdentidadEnum.DNI) {
      this.form.get("numeroDocumento").setValidators([
        Validators.required,
        Validators.pattern("^[0-9]{8,8}$"),
      ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.CE) {
      this.form.get("numeroDocumento").setValidators([
        Validators.required,
        Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
      ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.RUC) {
      this.form.get("numeroDocumento").setValidators([
        Validators.required,
        Validators.pattern("^[0-9]{11,11}$"),
      ]);
      this.form.updateValueAndValidity();
    } else {
      this.form.get("numeroDocumento").setValidators([
        Validators.required,
        Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
      ]);

      this.form.updateValueAndValidity();
    }
  }

  // Consulta RENIEC
  getReniecData(numeroDoc) {
    this.messageService.msgLoad("Consultado RENIEC...");
    this.pideService.getReniecByDni(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          let reniec = response.data;
          this.form.patchValue({ encargadoNombre: reniec.nombreCompleto });
        }
        else {
          var message = response.messages.join(",");
          let type = response.messageType;
          if (type == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          }
          else if (type == TYPE_MESSAGE.INFO) {
            this.messageService.msgWarning(message, () => { });
          }
          else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => {
        this.messageService.msgError(MESSAGES.API.ERR_SERVICE, () => { });
      }
    );
  }

  // Consulta MIGRACION
  getMigracionData(numeroDoc) {
    this.messageService.msgLoad("Consultado Migraciones...");
    this.pideService.getMigracionByNro(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        let message = response.messages.join(",");
        let type = response.messageType;
        if (response.success) {
          this.form.patchValue({ encargadoNombre: response.data.strNombreCompleto });
        }
        else {
          if (type == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          }
          else if (type == TYPE_MESSAGE.INFO) {
            this.messageService.msgWarning(message, () => { });
          }
          else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  // Buscar Encargado en REINEC o MIGRACION
  searchEncargado() {

    if (!this.form.get("encargadoNumeroDocumento").valid) {
      return;
    }

    const encargadoTipoDocumento = +this.form.get("encargadoTipoDocumento").value;
    if (encargadoTipoDocumento == null || encargadoTipoDocumento == 0) {
      return;
    }

    const encargadoNumeroDocumento = this.form.get("encargadoNumeroDocumento").value;

    if (encargadoTipoDocumento == this.tipoDocIdentidadEnum.DNI) {
      this.getReniecData(encargadoNumeroDocumento);
    }

    if (encargadoTipoDocumento == this.tipoDocIdentidadEnum.CE) {
      this.getMigracionData(encargadoNumeroDocumento);
    }

  }

  clearEncargado() {
    this.form.patchValue({
      encargadoTipoDocumento: null,
      encargadoNumeroDocumento: null,
      encargadoNombre: null
    });
    this.form.get("encargadoNumeroDocumento").clearValidators();
    this.form.get("encargadoNombre").clearValidators();
    this.form.get("encargadoNumeroDocumento").updateValueAndValidity();
    this.form.get("encargadoNombre").updateValueAndValidity();
  }

  onKeyUpNroEncargado(event) {
    this.form.patchValue({
      encargadoNombre: null
    });
  }

  // Select Tipo Doc Encargado
  selectedChangeTipoDocEncargado(tipoDocIdentidadId) {
    this.form.patchValue({ encargadoNumeroDocumento: null, encargadoNombre: null });
    if (tipoDocIdentidadId == this.tipoDocIdentidadEnum.DNI) {
      this.form.get("encargadoNumeroDocumento").setValidators([
        Validators.required,
        Validators.pattern("^[0-9]{8,8}$")
      ]);
      this.form.get("encargadoNombre").setValidators([
        Validators.required
      ]);
      this.form.get("encargadoNumeroDocumento").updateValueAndValidity();
      this.form.get("encargadoNombre").updateValueAndValidity();
    }
    else if (tipoDocIdentidadId == this.tipoDocIdentidadEnum.CE) {
      this.form.get("encargadoNumeroDocumento").setValidators([
        Validators.required,
        Validators.pattern("^[a-zA-Z0-9]{8,12}$")
      ]);
      this.form.get("encargadoNombre").setValidators([
        Validators.required
      ]);

      this.form.get("encargadoNumeroDocumento").updateValueAndValidity();
      this.form.get("encargadoNombre").updateValueAndValidity();
    }
    else {
      this.form.get("encargadoNumeroDocumento").clearValidators();
      this.form.get("encargadoNombre").clearValidators();
      this.form.get("encargadoNumeroDocumento").updateValueAndValidity();
      this.form.get("encargadoNombre").updateValueAndValidity();
    }
  }

  // Select Tipo Adquision
  selectedChangeTipoAdquision(tipo) {
    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();

    if (tipo == this.tipoAdquisicionEnum.SERVICIO) {
      this.hiddenTarifario = false;
      this.hiddenCatalogo = true;
    } else if (tipo == this.tipoAdquisicionEnum.BIEN) {
      this.hiddenCatalogo = false;
      this.hiddenTarifario = true;
    } else {
      this.hiddenCatalogo = true;
      this.hiddenTarifario = true;
    }
  }

  // Select Cuenta Corriente
  selectedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {

    this.form.patchValue({
      cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId,
      fechaDeposito: null,
      numeroDeposito: null,
      cuentaCorrienteValida: false,
      validarDeposito: this.validaDepositoEnum.PENDIENTE,
      depositoBancoDetalleId: null,
      importeDeposito: null
    });
  }

  // Select Tarifario
  selectedChangeTarifario(tarifario: Tarifario) {
    this.form.patchValue({ precioUnitario: tarifario.precio });
  }

  // Select Catalogo Bien
  selectedChangeCatalogoBien(catalogoBien: CatalogoBien) {

  }

  // Select Tipo Captación
  selectedChangeTipoCaptacion(id) {
    this.form.patchValue({
      cuentaCorrienteId: null,
      cuentaCorriente: null,
      validarDeposito: null,
      depositoBancoDetalleId: null,
      numeroDeposito: null,
      fechaDeposito: null,
      numeroCheque: null,
      cuentaCorrienteValida: false,
    });

    switch (id) {
      case this.tipoCaptacionEnum.EFECTIVO:
        this.hiddenDeposito = true;
        this.hiddenCheque = true;
        this.form.get("numeroDeposito").clearValidators();
        this.form.get("fechaDeposito").clearValidators();
        this.form.get("numeroCheque").clearValidators();
        break;
      case this.tipoCaptacionEnum.DEPOSITO_CUENTA_CORRRIENTE:
        this.hiddenDeposito = false;
        this.hiddenCheque = true;
        this.form.get("numeroDeposito").setValidators([
          Validators.required,
          Validators.pattern("^[A-Za-z0-9-]+$"),
        ]);
        this.form.get("fechaDeposito").setValidators([Validators.required]);
        this.form.get("numeroCheque").clearValidators();
        this.form.patchValue({
          validarDeposito: this.validaDepositoEnum.PENDIENTE,
          cuentaCorrienteValida: false,
        });

        break;
      case this.tipoCaptacionEnum.CHEQUE:
        this.hiddenCheque = false;
        this.hiddenDeposito = true;
        this.form.get("numeroDeposito").clearValidators();
        this.form.get("fechaDeposito").clearValidators();
        this.form.get("numeroCheque").setValidators([
          Validators.required,
          Validators.pattern("^[A-Za-z0-9-]+$"),
        ]);
        break;
      default:
        break;
    }

    this.form.get("numeroDeposito").updateValueAndValidity();
    this.form.get("fechaDeposito").updateValueAndValidity();
    this.form.get("numeroCheque").updateValueAndValidity();
  }

  // Select Fuente Origen
  selectedChangeFuenteOrigen(id) {
    if (id == this.fuenteOrigenEnum.INTERNO) {
      this.hiddenValidaFuente = false;
    }
    else {
      this.hiddenValidaFuente = true;
    }
  }

  // Validacion doc fuente
  validationDocFuente() {
    if (this.data.tipoDocumentoId == this.tipoDocEnum.NOTA_CREDITO || this.data.tipoDocumentoId == this.tipoDocEnum.NOTA_DEBITO) {

      this.hiddenDocFuente = false;

      this.form.get("fuenteTipoDocumento").setValidators([
        Validators.required
      ]);

      this.form.get("fuenteSerie").setValidators([
        Validators.required,
        Validators.pattern("^[a-zA-Z0-9]+$"),
        Validators.maxLength(VALIDATION.COMPROBANTE.FUENTE_SERIE_MAX_LENGTH)
      ]);

      this.form.get("fuenteCorrelativo").setValidators([
        Validators.required,
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(VALIDATION.COMPROBANTE.FUENTE_CORRELATIVO_MAX_LENGTH)
      ]);

      this.form.get("codigoTipoNota").setValidators([
        Validators.required
      ]);
      this.form.get("fuenteTipoDocumento").updateValueAndValidity();
      this.form.get("fuenteSerie").updateValueAndValidity();
      this.form.get("fuenteCorrelativo").updateValueAndValidity();
      this.form.get("codigoTipoNota").updateValueAndValidity();
    }
    else {
      this.hiddenDocFuente = true;
      this.form.get("fuenteTipoDocumento").clearValidators();
      this.form.get("fuenteSerie").clearValidators();
      this.form.get("fuenteCorrelativo").clearValidators();
      this.form.get("codigoTipoNota").clearValidators();
    }

  }

  // Buscar fuente
  searchDocFuente() {
    const fuenteOrigen = this.form.get("fuenteOrigen").value;
    const fuenteTipoDocumento = this.form.get("fuenteTipoDocumento").value;
    const fuenteSerie = this.form.get("fuenteSerie").value;
    const fuenteCorrelativo = this.form.get("fuenteCorrelativo").value;

    if (!this.form.get("fuenteOrigen").valid || fuenteOrigen == null)
      return;

    if (!this.form.get("fuenteTipoDocumento").valid || fuenteTipoDocumento == null)
      return;

    if (!this.form.get("fuenteSerie").valid || fuenteSerie == null)
      return;

    if (!this.form.get("fuenteCorrelativo").valid || fuenteCorrelativo == null)
      return;

    let comprobantePagoFuente = new ComprobantePagoFuente();
    comprobantePagoFuente.tipoComprobanteId = fuenteTipoDocumento;
    comprobantePagoFuente.serie = fuenteSerie;
    comprobantePagoFuente.correlativo = fuenteCorrelativo;
    this.comprobantePagoService.getComprobantePagoByFuente(comprobantePagoFuente).subscribe(
      response => {
        var message = response.messages.join(",");
        if (response.success) {
          this.form.patchValue({
            fuenteId: response.data.comprobantePagoId,
            fuenteValidar: this.fuenteValidaEnum.SI
          });
        }
        else {
          if (response.messageType == TYPE_MESSAGE.INFO) {
            this.messageService.msgInfo(message, () => { });
          } else if (response.messageType == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          } else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      error => {
        this.handleError(error);
      }
    );
  }

  // keyUp Deposito
  onKeyUpNroDeposito(event) {
    this.form.patchValue({
      cuentaCorrienteValida: false,
      validarDeposito: this.validaDepositoEnum.PENDIENTE,
      depositoBancoDetalleId: null,
      importeDeposito: null
    });
  }

  // Validar deposito banco
  validarDeposito() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.get("cuentaCorriente").valid ||
      !this.form.get("numeroDeposito").valid ||
      !this.form.get("fechaDeposito").valid) {
      return;
    }

    this.form.patchValue({
      cuentaCorrienteValida: false,
      validarDeposito: this.validaDepositoEnum.PENDIENTE,
      depositoBancoDetalleId: null,
      importeDeposito: null
    });

    const numeroDeposito = this.form.get("numeroDeposito").value;
    const fecha = this.form.get("fechaDeposito").value;

    let cuentaCorriente: CuentaCorriente = this.form.get("cuentaCorriente").value;
    const fechaDeposito = this.datePipe.transform(fecha, "yyyy-MM-dd");
    const cuentaCorrienteId = cuentaCorriente.cuentaCorrienteId;
    const clienteId = +this.form.get("clienteId").value;

    this.depositoBancoService.getDepositoBancoDetalleByNumeroFecha(
      numeroDeposito, fechaDeposito, cuentaCorrienteId, clienteId).subscribe((response) => {
        var message = response.messages.join(",");
        if (response.success) {
          this.onOpenAlertDepositoBanco("", "", response.data);
        } else {
          if (response.messageType == TYPE_MESSAGE.INFO) {
            this.messageService.msgInfo(message, () => { });
          } else if (response.messageType == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          } else {
            this.messageService.msgError(message, () => { });
          }
        }
      });
  }

  onOpenAlertDepositoBanco(messageTitle, messageIcon, depositoBancoDetalle: DepositoBancoDetalle) {
    const cuentaCorriente: CuentaCorriente = this.form.get("cuentaCorriente").value;
    const fechaDeposito = this.datePipe.transform(depositoBancoDetalle?.fechaDeposito, "dd-MM-yyyy");
    let deposito = {
      numeroCuenta: cuentaCorriente?.numero,
      numeroDeposito: depositoBancoDetalle?.numeroDeposito,
      importeDep: this.formatMoney(depositoBancoDetalle?.importe),
      fechaDeposito: fechaDeposito,
      tipoDocumento: "",
      numeroDocumento: "",
      fechaDocumento: ""
    }

    if (!depositoBancoDetalle.utilizado) {
      messageTitle = "Correcto!";
      messageIcon = "success";
      this.form.patchValue({
        cuentaCorrienteValida: true,
        validarDeposito: this.validaDepositoEnum.SI,
        depositoBancoDetalleId: depositoBancoDetalle.depositoBancoDetalleId,
        importeDeposito: depositoBancoDetalle.importe.toFixed(2)
      });
    } else {
      messageTitle = "Utilizado!";
      messageIcon = "warning";
      deposito.tipoDocumento = depositoBancoDetalle?.tipoDocumentoNombre;
      deposito.numeroDocumento = depositoBancoDetalle?.numeroDocumento;
      const fechaDocumento = this.datePipe.transform(depositoBancoDetalle?.fechaDocumento, "dd-MM-yyyy");
      if (fechaDocumento != null) {
        deposito.fechaDocumento = fechaDocumento;
      }
    }

    this.messageService.msgLgValidDeposito(messageTitle, messageIcon, deposito, () => { });
  }

  // Agregar el tarifario
  addDetalleTarifario() {

    const tarifario: Tarifario = this.form.get("tarifario").value;
    if (tarifario == null)
      return;

    if (!this.form.get("tipoAdquisicion").valid)
      return;

    if (!this.form.get("precioUnitario").valid)
      return;

    if (!this.form.get("cantidad").valid)
      return;

    if (!this.form.get("descuentoItem").valid)
      return;

    const tipoAdquisicion = this.form.get("tipoAdquisicion").value
    const precioUnitario = +this.form.get("precioUnitario").value
    const cantidad = +this.form.get("cantidad").value
    const descuentoItem = +this.form.get("descuentoItem").value
    const codigoTipoIGV = +this.form.get("codigoTipoIGV").value

    if (isNaN(codigoTipoIGV) || codigoTipoIGV == 0) {
      return;
    }


    let tarifarios = this.dataSource.data.filter(x => x.tarifarioId == tarifario.tarifarioId);
    if (tarifarios.length > 0) {
      this.messageService.msgWarning("El Tarifario ya se encuentra agregado", () => { });
      return;
    }

    let detalle = new ComprobantePagoDetalle();
    detalle.tipoAdquisicion = tipoAdquisicion;
    detalle.clasificadorIngresoId = tarifario.clasificadorIngresoId;
    detalle.tarifarioId = tarifario.tarifarioId;
    detalle.unidadMedida = "NIU";
    detalle.cantidad = cantidad;
    detalle.codigo = tarifario.codigo;
    detalle.descripcion = tarifario.nombre;
    detalle.codigoTipoMoneda = this.tipoMonedaEnum.NUEVO_SOL;
    detalle.precioUnitario = tarifario.precio;
    detalle.codigoTipoPrecio = this.tipoPrecioVentaEnum.PRECIO_UNITARIO;
    detalle.afectoIGV = false;
    detalle.igvItem = 0;
    detalle.codigoTipoIGV = codigoTipoIGV;
    detalle.descuentoItem = descuentoItem;
    detalle.descuentoTotal = detalle.descuentoItem * detalle.cantidad;
    detalle.factorDescuento = 0;
    detalle.subTotal = 0;
    detalle.valorVenta = 0;
    detalle.ingresoPecosaDetalleId = null;
    detalle.serieFormato = "";
    detalle.serieDel = 0;
    detalle.serieAl = 0;
    detalle.estado = "1";

    if (detalle.precioUnitario <= detalle.descuentoItem) {
      this.messageService.msgWarning("El descuento debe ser menor al precio unitario ", () => { });
      return;
    }

    if (detalle.codigoTipoIGV == this.tipoIGVEnum.GRAVADA) {
      detalle.precioSinIGV = Tools.basePrecio(detalle.precioUnitario, this.IGVEnum.VALOR);
      detalle.totalItemSinIGV = detalle.precioSinIGV * cantidad;
      detalle.valorVenta = (detalle.precioUnitario - detalle.descuentoItem) * detalle.cantidad;
      detalle.subTotal = Tools.baseTotal(detalle.valorVenta, this.IGVEnum.VALOR);
      detalle.igvItem = Tools.itemTotalIGV(detalle.valorVenta, detalle.subTotal);
      detalle.afectoIGV = true;
      detalle.codigoTipoPrecio = this.tipoPrecioVentaEnum.PRECIO_UNITARIO;
    }

    if (detalle.codigoTipoIGV == this.tipoIGVEnum.EXONERADA) {
      detalle.subTotal = (detalle.precioUnitario - detalle.descuentoItem) * detalle.cantidad;
      detalle.valorVenta = detalle.subTotal;
    }

    if (detalle.codigoTipoIGV == this.tipoIGVEnum.INAFECTA) {
      detalle.subTotal = (detalle.precioUnitario - detalle.descuentoItem) * detalle.cantidad;
      detalle.valorVenta = detalle.subTotal;
    }

    this.dataSource.data.push(detalle);
    this.dataSource._updateChangeSubscription();

    this.form.patchValue({
      totalDescuento: this.getTotalDescuento().toFixed(2),
      importeBruto: this.getImporteBruto().toFixed(2),
      importeTotal: this.getImporteTotal().toFixed(2),
      totalOpGravada: this.getTotalGravada().toFixed(2),
      igvTotal: this.getTotalIGV().toFixed(2),
      totalOpInafecta: this.getTotalInafecta().toFixed(2),
      totalOpExonerada: this.getTotalExonerada().toFixed(2),
      tarifario: null,
      precioUnitario: 0.0,
      cantidad: 1,
      descuentoItem: 0.0,
      codigoTipoIGV: this.tipoIGVEnum.INAFECTA
    });
  }

  deleteDataSource(data: ComprobantePagoDetalle) {
    this.dataSource.data = this.dataSource.data.filter((obj) => obj !== data);
    this.dataSource._updateChangeSubscription();
  }
  // Agregar lista de Catalogos
  openDialogCatalogoBien() {

    if (!this.form.get("tipoAdquisicion").valid)
      return;

    const catalogoBien: CatalogoBien = this.form.get("catalogoBien").value;
    if (catalogoBien == null)
      return;

    let catalogos = this.dataSource.data.filter(x => x.catalogoBienId == catalogoBien.catalogoBienId);
    if (catalogos.length > 0) {
      this.messageService.msgWarning("El Catálogo de Bien ya se encuentra agregado", () => { });
      return;
    }
    
    const dialogRef = this.dialog.open(ListSaldoIngresoNotaDebitoComponent, {
      width: "1000px",
      disableClose: true,
      data: catalogoBien,
    });

    dialogRef.afterClosed().subscribe((response: IngresoPecosaDetalle[]) => {
      if (response.length > 0) {
        response.filter(x => x.cantidadSalida > 0 && x.serieAlSalida > 0 && x.serieAlSalida > 0).forEach((item) => {
          let detalle = new ComprobantePagoDetalle();
          detalle.tipoAdquisicion = this.form.get("tipoAdquisicion").value;
          detalle.clasificadorIngresoId = catalogoBien.clasificadorIngresoId;
          detalle.catalogoBienId = catalogoBien.catalogoBienId;
          detalle.unidadMedida = catalogoBien.unidadMedida.abreviatura;
          detalle.cantidad = item.cantidadSalida;
          detalle.codigo = catalogoBien.codigo;
          let descripcion = catalogoBien.descripcion + " " + item?.serieFormato + " " + item?.serieDelSalida + "-" + item?.serieAlSalida;
          detalle.descripcion = descripcion;
          detalle.codigoTipoMoneda = this.tipoMonedaEnum.NUEVO_SOL;
          detalle.precioUnitario = item.precioUnitario;
          detalle.precioSinIGV = 0.0;
          detalle.totalItemSinIGV = 0.0;
          detalle.codigoTipoPrecio = this.tipoPrecioVentaEnum.VALOR_REFERENCIAL;
          detalle.afectoIGV = false;
          detalle.igvItem = 0.00;
          detalle.codigoTipoIGV = this.tipoIGVEnum.INAFECTA;
          detalle.descuentoItem = 0.0;
          detalle.descuentoTotal = 0.0;
          detalle.factorDescuento = 0.0;
          detalle.subTotal = 0.0;
          detalle.valorVenta = 0.0;
          detalle.ingresoPecosaDetalleId = item.ingresoPecosaDetalleId;
          detalle.serieFormato = item.serieFormato;
          detalle.serieDel = item.serieDelSalida;
          detalle.serieAl = item.serieAlSalida;
          detalle.estado = "1";

          if (detalle.codigoTipoIGV == this.tipoIGVEnum.INAFECTA) {
            detalle.subTotal = (detalle.precioUnitario - detalle.descuentoItem) * detalle.cantidad;
            detalle.valorVenta = detalle.subTotal;
          }

          this.dataSource.data.push(detalle);
          this.dataSource._updateChangeSubscription();
        });
      }
    });

    this.form.patchValue({
      totalDescuento: this.getTotalDescuento().toFixed(2),
      importeBruto: this.getImporteBruto().toFixed(2),
      importeTotal: this.getImporteTotal().toFixed(2),
      totalOpGravada: this.getTotalGravada().toFixed(2),
      igvTotal: this.getTotalIGV().toFixed(2),
      totalOpInafecta: this.getTotalInafecta().toFixed(2),
      totalOpExonerada: this.getTotalExonerada().toFixed(2),
      catalogoBien: null,
      precioUnitario: 0.0,
      cantidad: 1,
      descuentoItem: 0.0,
      codigoTipoIGV: null
    });

  }

  // Importe Total 
  getImporteTotal() {
    return this.dataSource.data.map((t) => t.valorVenta).reduce((acc, value) => acc + value, 0);
  }

  // Total Gravada
  getImporteBruto() {
    return this.dataSource.data.map((t) => t.subTotal).reduce((acc, value) => acc + value, 0);
  }

  // Total Gravada
  getTotalGravada() {
    return this.dataSource.data.filter(x => x.codigoTipoIGV == this.tipoIGVEnum.GRAVADA).map((t) => t.subTotal).reduce((acc, value) => acc + value, 0);
  }

  // Total Inafecta
  getTotalInafecta() {
    return this.dataSource.data.filter(x => x.codigoTipoIGV == this.tipoIGVEnum.INAFECTA).map((t) => t.subTotal).reduce((acc, value) => acc + value, 0);
  }

  // Total Exonerada
  getTotalExonerada() {
    return this.dataSource.data.filter(x => x.codigoTipoIGV == this.tipoIGVEnum.EXONERADA).map((t) => t.subTotal).reduce((acc, value) => acc + value, 0);
  }

  // Total IGV
  getTotalIGV() {
    return this.dataSource.data.filter(x => x.codigoTipoIGV == this.tipoIGVEnum.GRAVADA).map((t) => t.igvItem).reduce((acc, value) => acc + value, 0);
  }

  // Total Descuento
  getTotalDescuento() {
    return this.dataSource.data.map((t) => t.descuentoTotal).reduce((acc, value) => acc + value, 0);
  }

  // Guardar comprobante
  onSubmit(form: ComprobantePago) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle de la Nota de Débito", () => { });
      return;
    }
    let comprobantepago = form;

    if (comprobantepago.tipoDocumentoId == this.tipoDocEnum.NOTA_CREDITO && comprobantepago.fuenteOrigen == this.fuenteOrigenEnum.INTERNO) {
      if (comprobantepago.fuenteValidar != this.fuenteValidaEnum.SI) {
        this.messageService.msgWarning("Se debe validar el documento fuente a modificar", () => { });
        return;
      }
    }

    comprobantepago.importeBruto = +this.getImporteBruto();
    comprobantepago.valorIGV = +form.valorIGV;
    comprobantepago.igvTotal = +this.getTotalIGV();
    comprobantepago.iscTotal = +form.iscTotal;
    comprobantepago.otrTotal = +form.otrTotal;
    comprobantepago.otrcTotal = +form.otrcTotal;
    comprobantepago.importeTotal = +this.getImporteTotal();
    comprobantepago.totalOpGravada = +this.getTotalGravada();
    comprobantepago.totalOpInafecta = +this.getTotalInafecta();
    comprobantepago.totalOpExonerada = +this.getTotalExonerada();
    comprobantepago.totalOpGratuita = 0;
    comprobantepago.totalDescuento = this.getTotalDescuento();

    comprobantepago.comprobantePagoDetalle = this.dataSource.data;

    this.messageService.msgConfirm(MESSAGES.FORM.COMPROBANTE_PAGO.GENERAR,
      () => {
        this.messageService.msgLoad("Generando Comprobante...");
        this.comprobantePagoService.createComprobantePago(comprobantepago).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {

      });
  }

  handleResponse(type, message, success) {
    if (success) {
      this.messageService.msgSuccess(message, () => {
        this.onNoClick();
      });
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { });
      }
      else {
        this.messageService.msgError(message, () => { });
      }
    }
  }

  handleError(error) {
    throw error;
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onCloseDialog() {
    this.messageService.msgClose(MESSAGES.FORM.CLOSE_FORM, () => {
      this.onNoClick();
    });
  }

  onNoClick(): void {
    this.dialogRef.close(0);
  }

  formatMoney(i) {
    return Tools.formatMoney(i);
  }

  // Nuevo Cliente
  openDialogNewCliente() {
    let cliente = new Cliente();
    cliente.usuarioCreador = this.data.usuarioCreador;
    const dialogRef = this.dialog.open(NewNotaDebitoClienteComponent, {
      width: "800px",
      disableClose: true,
      data: cliente,
    });

    dialogRef.afterClosed().subscribe((response: Cliente) => {
      if (response?.clienteId > 0) {
        this.form.patchValue({
          clienteId: response?.clienteId,
          tipoDocumentoIdentidadId: response?.tipoDocumentoIdentidadId,
          numeroDocumento: response?.numeroDocumento,
          nombreCliente: response?.nombre,
        });

        this.valiationTipoDocIdentidad(response?.tipoDocumentoIdentidadId)
      }
    });
  }
}
