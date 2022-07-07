import {
  Component,
  OnInit,
  Inject,
  DEFAULT_CURRENCY_CODE,
} from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { TipoReciboIngreso } from "src/app/core/models/tiporeciboingreso";
import { FuenteFinanciamiento } from "src/app/core/models/fuentefinanciamiento";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import {
  FormGroup,
  FormBuilder,
  Validators,
} from "@angular/forms";
import {
  ReciboIngreso,
  ReciboIngresoDetalle,
} from "src/app/core/models/reciboingreso";
import { ClienteService } from "src/app/core/services/cliente.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { ReciboIngresoService } from "src/app/core/services/recibo-ingreso.service";
import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { TipoReciboIngresoService } from "src/app/core/services/tipo-recibo-ingreso.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { DepositoBancoService } from "src/app/core/services/deposito-banco.service";
import { DatePipe } from "@angular/common";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from "@angular/material/core";
import {
  MAT_MOMENT_DATE_ADAPTER_OPTIONS,
  MomentDateAdapter,
} from "@angular/material-moment-adapter";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { TipoCaptacionService } from "src/app/core/services/tipo-captacion.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { DepositoBancoDetalle } from "src/app/core/models/depositobanco";
import { TipoCaptacion } from "src/app/core/models/tipocaptacion";
import { ValidaDepositoEnum, TipoDocIdentidadEnum, TipoCaptacionEnum, TipoReciboIngresoEnum, EstadoDepositoBancoDetalleEnum } from "src/app/core/enums/recaudacion.enum";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { Tools } from "src/app/core/utils/tools";

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
  selector: "app-edit-recibo-ingreso",
  templateUrl: "./edit-recibo-ingreso.component.html",
  styleUrls: ["./edit-recibo-ingreso.component.scss"],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
    { provide: DEFAULT_CURRENCY_CODE, useValue: "PEN" },
  ],
})
export class EditReciboIngresoComponent implements OnInit {
  //Enums
  validaDepositoEnum = ValidaDepositoEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoCaptacionEnum = TipoCaptacionEnum;
  tipoReciboIngresoEnum = TipoReciboIngresoEnum;
  estadoDepositoBancoDetalleEnum = EstadoDepositoBancoDetalleEnum;

  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  tipoReciboIngresos: TipoReciboIngreso[] = [];

  fuenteFinanciamientos: FuenteFinanciamiento[] = [];

  clasificadorIngresos: ClasificadorIngreso[] = [];
  filteredClasificadorIngresos;



  reciboIngresoDetalles: ReciboIngresoDetalle[] = [];

  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];

  tipoCaptaciones: TipoCaptacion[] = [];
  displayedColumns: string[] = [
    "index",
    "clasificadorIngreso",
    "importe",
    "actions",
  ];

  dataSource = new MatTableDataSource(this.reciboIngresoDetalles);

  hidenDeposito = false;
  hidenCheque = false;
  depositoIsValid = false;
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditReciboIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ReciboIngreso,
    private clienteService: ClienteService,
    private cuentaCorrienteService: CuentaCorrienteService,
    private reciboIngresoService: ReciboIngresoService,
    private fuenteFinanciamientoService: FuenteFinanciamientoService,
    private tipoReciboIngresoServie: TipoReciboIngresoService,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private depositoBancoService: DepositoBancoService,
    private tipoCaptacionService: TipoCaptacionService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private fb: FormBuilder,
    private datePipe: DatePipe,
    private dialog: MatDialog,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      reciboIngresoId: [data?.reciboIngresoId],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      tipoReciboIngresoId: [data?.tipoReciboIngresoId, Validators.compose([Validators.required])],
      clienteId: [data?.clienteId],
      cuentaCorrienteId: [data?.cuentaCorrienteId],
      fuenteFinanciamientoId: [data?.fuenteFinanciamientoId, Validators.compose([Validators.required])],
      registroLineaId: [data?.registroLineaId],
      tipoDocumentoId: [data?.tipoDocumentoId],
      numero: [data.numero],
      fechaEmision: [data.fechaEmision],
      tipoCaptacionId: [data.tipoCaptacionId, Validators.compose([Validators.required]),],
      depositoBancoDetalleId: [data.depositoBancoDetalleId],
      importeTotal: [data.importeTotal,
      Validators.compose([
        Validators.required,
        Validators.pattern(/^[.\d]+$/),
        Validators.min(0),
        Validators.maxLength(12)
      ])
      ],
      numeroDeposito: [data?.numeroDeposito,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(15)
      ])
      ],
      fechaDeposito: [data?.fechaDeposito],
      validarDeposito: [data?.validarDeposito],
      numeroCheque: [data?.numeroCheque,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      numeroOficio: [data?.numeroOficio,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      numeroComprobantePago: [data?.numeroComprobantePago,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      expedienteSiaf: [data?.expedienteSiaf,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      numeroResolucion: [data?.numeroResolucion,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      cartaOrden: [data?.cartaOrden,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      liquidacionIngreso: [data?.liquidacionIngreso,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      papeletaDeposito: [data?.papeletaDeposito,
      Validators.compose([
        Validators.pattern('^[A-Za-z0-9-]+$'),
        Validators.maxLength(25)
      ])
      ],
      concepto: [data?.concepto,
      Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
        Validators.maxLength(300)
      ])
      ],
      referencia: [data?.referencia,
      Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
        Validators.maxLength(300)
      ])
      ],
      liquidacionId: [data?.liquidacionId],
      estado: [data?.estado],
      usuarioModificador: [data?.usuarioModificador],

      // Cliente
      tipoDocumentoIdentidadId: [
        data?.cliente?.tipoDocumentoIdentidadId,
        Validators.compose([Validators.required]),
      ],
      numeroDocumento: [
        data?.cliente?.numeroDocumento,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(12)
        ]),
      ],
      nombreCliente: [data?.cliente?.nombre],
      // Extra
      cuentaCorriente: [data?.cuentaCorriente, Validators.compose([Validators.required])],
      // Detalle
      clasificadorIngreso: [null],
      importe: [0,
        Validators.compose([
          Validators.pattern(/^[.\d]+$/),
          Validators.min(0),
          Validators.maxLength(12)
        ])
      ],
    });
  }

  ngOnInit() {
    this.onLoadMaestras();
    this.onloadDetalle();

    this.selectedTipoCaptacion(this.data.tipoCaptacionId);
  }

  onloadDetalle() {
    this.reciboIngresoService.getReciboIngresoById(this.data.reciboIngresoId).subscribe(
      (response) => {
        if (response.success) {
          this.dataSource.data = response.data.reciboIngresoDetalle;
          this.dataSource._updateChangeSubscription();
        }
      }
    );
  }

  onLoadMaestras() {

    // Tipo Recibo  Ingresos
    this.tipoReciboIngresoServie.getTipoReciboIngresos()
      .subscribe((response) => {
        if (response.success) {
          if (this.data.liquidacionId > 0 || this.data.liquidacionId != null) {
            this.tipoReciboIngresos = response.data;
          }
          else{
            this.tipoReciboIngresos = response.data.filter(x => x.tipoReciboIngresoId != this.tipoReciboIngresoEnum.CAPTACION_VENTANILLA);
          }

        }
      });

    //Fuente Financiamiento
    this.fuenteFinanciamientoService
      .getFuenteFinanciamientos()
      .subscribe((response) => {
        if (response.success) {
          this.fuenteFinanciamientos = response.data;
        }
      });

    // Tipo Captaciones
    this.tipoCaptacionService.getTipoCaptaciones().subscribe((response) => {
      if (response.success) {
        if (this.data.liquidacionId > 0 || this.data.liquidacionId != null) {
          this.tipoCaptaciones = response.data;
        }
        else {
          this.tipoCaptaciones = response.data.filter(x => x.tipoCaptacionId != this.tipoCaptacionEnum.VARIOS);
        }
      }
    });

    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService
      .getTipoDocumentoIdentidades()
      .subscribe((response) => {
        if (response.success) {
          this.tipoDocumentoClientes = response.data;
        }
      });

    this.cuentaCorrienteService
      .getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.numeroDenominacion = element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();
          let cuentas = this.cuentasCorrientes.filter(x => x.cuentaCorrienteId == this.data.cuentaCorrienteId)
          if (cuentas.length > 0) {
            this.form.patchValue({ cuentaCorriente: cuentas[0] });
          }
        }
      });

    // Clasificador de Ingresos
    this.clasificadorIngresoService
      .getClasificadorIngresos()
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.descripcion = element?.codigo + " - " + element?.descripcion;
          });
          this.clasificadorIngresos = response.data;
          this.filteredClasificadorIngresos = this.clasificadorIngresos.slice();
        }
      });
  }

  selectedTipoDocIdentidad(id: number) {
    this.form.patchValue({
      numeroDocumento: null,
      clienteNombre: null,
    });

    this.valiationTipoDocIdentidad(id);
  }

  valiationTipoDocIdentidad(id: number) {
    if (id == this.tipoDocIdentidadEnum.DNI) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{8,8}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.CE) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.RUC) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{11,11}$"),
        ]);
      this.form.updateValueAndValidity();
    } else {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    }
  }

  searchNroDoc() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.get("tipoDocumentoIdentidadId").valid || !this.form.get("numeroDocumento").valid) {
      return;
    }

    const tipoDocumentoIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;
    const numeroDocumento = this.form.get("numeroDocumento").value;

    this.clienteService.getClienteByTipoNroDocumento(tipoDocumentoIdentidadId, numeroDocumento)
      .subscribe((response) => {
        var message = response.messages.join(",");
        if (response.success) {
          let cliente = response.data;
          this.form.patchValue({ nombreCliente: cliente?.nombre, clienteId: cliente?.clienteId });
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

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {
    this.form.patchValue({ cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId });
  }
  // Clasificador Ingreso
  seletedClasificadorIngresoOption(clasificadorIngreso: ClasificadorIngreso) {
  }

  seletedTipoReciboIngreso(id) {

  }

  selectedTipoCaptacion(id) {
    switch (id) {
      case this.tipoCaptacionEnum.EFECTIVO:
        this.hidenDeposito = false;
        this.hidenCheque = false;
        this.form.get("numeroDeposito").clearValidators();
        this.form.get("fechaDeposito").clearValidators();
        this.form.get("numeroCheque").clearValidators();
        break;
      case this.tipoCaptacionEnum.DEPOSITO_CUENTA_CORRRIENTE:
        this.hidenDeposito = true;
        this.hidenCheque = false;
        this.form.get("numeroDeposito").setValidators([
          Validators.required,
          Validators.pattern("^[A-Za-z0-9-]+$"),
        ]);
        this.form.get("fechaDeposito").setValidators([Validators.required]);
        this.form.get("numeroCheque").clearValidators();

        break;
      case this.tipoCaptacionEnum.CHEQUE:
        this.hidenCheque = true;
        this.hidenDeposito = false;
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

  addRowData() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let clasificador: ClasificadorIngreso = this.form.get("clasificadorIngreso").value;

    if (!clasificador) {
      this.messageService.msgWarning(
        "Seleccione el clasificador de Ingreso",
        () => { }
      );
      return;
    }

    var importe = +this.form.get("importe").value;

    if (!this.form.get("importe").valid || importe == 0) {
      this.messageService.msgWarning(
        "Ingrese el importe correctamente",
        () => { }
      );
      return;
    }

    let data = this.dataSource.data.filter(x => x.clasificadorIngresoId == clasificador.clasificadorIngresoId);

    if (data.length > 0) {
      this.messageService.msgWarning("El clasificador de ingreseo ya se encuetra agregado", () => { });
      return;
    }

    let reciboIngresoDetalle = new ReciboIngresoDetalle();
    reciboIngresoDetalle.clasificadorIngreso = clasificador;
    reciboIngresoDetalle.clasificadorIngresoId = clasificador.clasificadorIngresoId;
    reciboIngresoDetalle.importe = importe;
    reciboIngresoDetalle.referencia = " ";
    reciboIngresoDetalle.usuarioCreador = this.data.usuarioCreador;
    this.dataSource.data.push(reciboIngresoDetalle);
    this.dataSource._updateChangeSubscription();

    this.form.patchValue({ clasificadorIngreso: null, importe: 0.0, });

  }

  deleteRowData(data: ReciboIngresoDetalle) {
    this.dataSource.data = this.dataSource.data.filter((obj) => obj !== data);
  }

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
      this.form.patchValue({ validarDeposito: this.validaDepositoEnum.SI, depositoBancoDetalleId: depositoBancoDetalle.depositoBancoDetalleId });
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

  getTotal() {
    return this.dataSource.data.map((t) => t.importe).reduce((acc, value) => acc + value, 0);
  }

  onSubmit(form: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle del recibo de ingreso", () => {
      });
      return;
    }

    let total = this.dataSource.data.map((t) => t.importe).reduce((acc, value) => acc + value, 0);

    if (total != (+form.importeTotal)) {
      this.messageService.msgWarning("El importe total del detalle debe ser igual al importe total ingresado", () => { });
      return;
    }

    let reciboIngreso = new ReciboIngreso();
    reciboIngreso = form;
    reciboIngreso.importeTotal = +reciboIngreso.importeTotal;
    reciboIngreso.reciboIngresoDetalle = this.dataSource.data;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.reciboIngresoService.updateReciboIngreso(reciboIngreso).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {

      });
  }

  //Response
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
}
