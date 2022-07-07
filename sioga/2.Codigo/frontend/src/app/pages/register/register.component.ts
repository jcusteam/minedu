import { DatePipe } from "@angular/common";
import { Component, OnInit, ViewChild, HostListener, ViewChildren, QueryList, } from "@angular/core";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from "@angular/material-moment-adapter";
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS, } from "@angular/material/core";
import { Router, ActivatedRoute, NavigationEnd, ActivatedRouteSnapshot, UrlSegment } from "@angular/router";
import { Title } from "@angular/platform-browser";
import { PerfectScrollbarDirective } from "ngx-perfect-scrollbar";
import { AppSettings } from "../../app.settings";
import { Settings } from "../../app.settings.model";
import { MatTableDataSource } from "@angular/material/table";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { RegistroLinea, RegistroLineaDetalle } from "src/app/core/models/registrolinea";
import { Banco } from "src/app/core/models/banco";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { TipoReciboIngreso } from "src/app/core/models/tiporeciboingreso";
import { RecaudacionService } from "src/app/core/services/recaudacion.service";
import { MessageService } from "src/app/core/services/message.service";
import { Cliente } from "src/app/core/models/cliente";
import { environment } from "src/environments/environment";
import { EstadoRegistroLineaEnum, LabelTipDocIdentidadEnum, TipoDocEnum, TipoDocIdentidadEnum, TipoReciboIngresoEnum, ValidaDepositoEnum } from "src/app/core/enums/sioga.enum";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

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
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: ["./register.component.scss"],
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
export class RegisterComponent implements OnInit {

  @ViewChild("backToTop") backToTop: any;
  @ViewChildren(PerfectScrollbarDirective)
  pss: QueryList<PerfectScrollbarDirective>;
  public settings: Settings;
  public lastScrollTop: number = 0;
  public showBackToTop: boolean = false;
  public pageTitle: string;
  public breadcrumbs: {
    name: string;
    url: string;
  }[] = [];

  // Enum
  tipoDocEnum = TipoDocEnum;
  estadoRegistroLineaEnum = EstadoRegistroLineaEnum;
  tipoReciboIngresoEnum = TipoReciboIngresoEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  labelTipDocIdentidadEnum = LabelTipDocIdentidadEnum;
  validaDepositoEnum = ValidaDepositoEnum;
  startDate = new Date();

  tipoDocumentos = [];

  form: FormGroup;
  bancos: Banco[] = [];
  labelNombreCliente = "Nombres y Apellidos";
  hiddenExpedienteESinad = false;

  importeTotalDetalle: number = 0;
  clasificadorIngresos: ClasificadorIngreso[] = [];
  filteredClasificadorIngresos;

  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  tipoRecibosIngresos: TipoReciboIngreso[] = [];

  displayedColumns: string[] = [
    "nro",
    "clasificadorIngreso",
    "importe",
    "actions",
  ];

  registroLineaDetalles: RegistroLineaDetalle[] = [];
  dataSource = new MatTableDataSource(this.registroLineaDetalles);

  siteKey: string = environment.siteKeyRecaptcha;
  isCAPTCHA = false;

  constructor(
    public appSettings: AppSettings,
    public router: Router,
    public activatedRoute: ActivatedRoute,
    public title: Title,
    private recaudaService: RecaudacionService,
    private fb: FormBuilder,
    private messageService: MessageService
  ) {
    this.settings = this.appSettings.settings;

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.breadcrumbs = [];
        this.parseRoute(this.router.routerState.snapshot.root);
        this.pageTitle = "";
        this.breadcrumbs.forEach((breadcrumb) => {
          this.pageTitle += " > " + breadcrumb.name;
        });
        this.title.setTitle(this.settings.name + this.pageTitle);
      }
    });

    // Form
    this.form = this.fb.group({
      registroLineaId: [0],
      unidadEjecutoraId: [0, Validators.compose([Validators.required])],
      cuentaCorrienteId: [0, Validators.compose([Validators.required])],
      bancoId: [null, Validators.compose([Validators.required])],
      clienteId: [0],
      tipoDocumentoId: [this.tipoDocEnum.REGISTRO_LINEA, Validators.compose([Validators.required])],
      numero: ["0000"],
      fechaRegistro: [new Date()],
      tipoReciboIngresoId: [null, Validators.compose([Validators.required])],
      numeroDeposito: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-z0-9 -]+$'),
          Validators.maxLength(12)
        ])
      ],
      importeDeposito: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^[.\d]+$/),
          Validators.maxLength(12)
        ])
      ],
      fechaDeposito: [null, Validators.compose([Validators.required])],
      validarDeposito: [this.validaDepositoEnum.PENDIENTE],
      numeroOficio: [null,
        Validators.compose([
          Validators.pattern('^[A-Za-z0-9 -]+$'),
          Validators.maxLength(25)
        ])
      ],
      numeroComprobantePago: [null,
        Validators.compose([
          Validators.pattern('^[A-Za-z0-9 -]+$'),
          Validators.maxLength(25)
        ])
      ],
      expedienteSiaf: [null,
        Validators.compose([
          Validators.pattern('^[A-Za-z0-9 -]+$'),
          Validators.maxLength(25)
        ])
      ],
      numeroResolucion: [null,
        Validators.compose([
          Validators.pattern('^[A-Za-z0-9 -]+$'),
          Validators.maxLength(25)
        ])
      ],
      expedienteESinad: [null,
        Validators.compose([
          Validators.pattern('^[A-Za-z0-9 -]+$'),
          Validators.maxLength(30)
        ])
      ],

      observacion: ["",
        Validators.compose([
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
          Validators.maxLength(300)
        ])
      ],
      estado: [this.estadoRegistroLineaEnum.EMITIDO, Validators.compose([Validators.required])],
      usuarioCreador: ["REGISTRO-LINEA", Validators.compose([Validators.required])],

      // Cliente
      tipoDocumentoIdentidadId: [null, Validators.compose([Validators.required])],
      numeroDocumento: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]+$"),
          Validators.maxLength(12)
        ]),
      ],
      clienteNombre: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
          Validators.maxLength(200)
        ]),
      ],
      correo: [null,
        Validators.compose([
          Validators.required,
          Validators.email,
          Validators.maxLength(100)
        ]),
      ],
      // Extra
      cuentaCorriente: [null, Validators.compose([Validators.required])],
      // Detalle
      clasificadorIngreso: [null],
      importe: [0, Validators.compose([Validators.pattern(/^[.\d]+$/)])],
    });
  }

  ngOnInit() {
    this.loadMaestras();
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.settings.loadingSpinner = false;
    }, 300);
    this.backToTop.nativeElement.style.display = "none";
  }

  resolved(captchaResponse: string) {
    this.isCAPTCHA = true;
  }


  loadMaestras() {

    this.recaudaService.getBancos().subscribe(
      (response) => {
        if (response.success) {
          this.bancos = response.data;
        }
      });

    this.recaudaService.getTipoDocIdentidades().subscribe((response) => {
      if (response.success) {
        this.tipoDocumentos = response.data;
      }
    });

    this.recaudaService.getTipoReciboIngresos().subscribe(
      (response) => {
        if (response.success) {
          this.tipoRecibosIngresos = response.data.filter(
            (obj) =>
              obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.HABILITACION_HURBANA ||
              obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.DEVOLUCION_SALGO_VIATICO ||
              obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.DEPOSITO_INDEBIDO
          );
        }
      }
    );

    this.recaudaService.getCuentaCorrientes().subscribe((response) => {
      if (response.success) {
        response.data.forEach((element) => {
          element.denominacion =
            element?.numero + " - " + element?.denominacion;
        });
        this.cuentasCorrientes = response.data;
        this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();

      }
    });

    this.recaudaService.getClasificadorIngresos().subscribe((response) => {
      if (response.success) {
        response.data.forEach((element) => {
          element.descripcion =
            element?.codigo + " - " + element?.descripcion;
        });
        this.clasificadorIngresos = response.data;
        this.filteredClasificadorIngresos = this.clasificadorIngresos.slice();
      }
    });
  }


  addRowData() {
    let clasificador: ClasificadorIngreso = this.form.get("clasificadorIngreso").value;

    if (!clasificador) {
      this.messageService.msgWarning("Seleccione el clasificador de Ingreso", () => { });
      return;
    }

    var importe = +this.form.get("importe").value;

    if (!this.form.get("importe").valid || importe == 0) {
      this.messageService.msgWarning("Ingrese el importe correctamente", () => { });
      return;
    }

    let data = this.dataSource.data.filter(x => x.clasificadorIngresoId == clasificador.clasificadorIngresoId);

    if (data.length > 0) {
      this.messageService.msgWarning("El clasificador de ingreseo ya se encuetra agregado.", () => { });
      return;
    }

    let registroLineaDetalle = new RegistroLineaDetalle();
    registroLineaDetalle.clasificadorIngreso = clasificador;
    registroLineaDetalle.clasificadorIngresoId = clasificador.clasificadorIngresoId;
    registroLineaDetalle.importe = importe;
    registroLineaDetalle.referencia = " ";
    registroLineaDetalle.estado = "1";
    this.dataSource.data.push(registroLineaDetalle);
    this.dataSource._updateChangeSubscription();

    this.form.patchValue({
      clasificadorIngreso: null,
      importe: 0.0,
    });

    this.updateTotalImporte();
  }

  deleteRowData(data) {
    this.dataSource.data = this.dataSource.data.filter((obj) => obj !== data);
    this.updateTotalImporte();
  }

  updateTotalImporte() {
    this.importeTotalDetalle = this.getImporteTotal();
  }

  getImporteTotal(): number {
    return this.dataSource.data.map((t) => t.importe).reduce((acc, value) => acc + value, 0);
  }

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {
    this.form.patchValue({ cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId });
    this.form.patchValue({ unidadEjecutoraId: cuentaCorriente.unidadEjecutoraId });
  }

  // Clasificador Ingreso
  seletedChangeClasificadorIngreso(clasificadorIngreso: ClasificadorIngreso) {

  }

  onSelectionTipoDocIdentidad(id: number) {
    this.form.patchValue({
      numeroDocumento: null,
      clienteNombre: null,
      correo: null,
      direccion: null
    });

    if (id == this.tipoDocIdentidadEnum.RUC) {
      this.labelNombreCliente = this.labelTipDocIdentidadEnum.RAZON_SOCIAL;
    }
    else {
      this.labelNombreCliente = this.labelTipDocIdentidadEnum.NOMBRE_APELLIDO;
    }

    this.valiationTipoDocIdentidad(id);

  }

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

  selectedChangeTipoRecibo(id) {
    if (id == this.tipoReciboIngresoEnum.DEPOSITO_INDEBIDO) {
      this.hiddenExpedienteESinad = true;
      this.form.get("expedienteESinad").setValidators(Validators.compose([Validators.required]));
      this.form.get("expedienteESinad").updateValueAndValidity();
    }
    else {
      this.hiddenExpedienteESinad = false;
      this.form.get("expedienteESinad").clearValidators();
      this.form.get("expedienteESinad").updateValueAndValidity();
    }
  }
  onSubmit(form: RegistroLinea) {

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => {
      });
      return;
    }


    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle del Registro en Linea.", () => {
      });
      return;
    }

    if (!this.isCAPTCHA) {
      this.messageService.msgWarning(MESSAGES.FORM.REGISTRO_LINEA.ERROR_CAPTCHA, () => {
      });
      return;
    }

    //Cliente
    let cliente = new Cliente();
    cliente.tipoDocumentoIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;
    cliente.numeroDocumento = this.form.get("numeroDocumento").value;
    cliente.nombre = this.form.get("clienteNombre").value;
    cliente.direccion = "";
    cliente.correo = this.form.get("correo").value;
    cliente.usuarioCreador = "REGISTRO-LINEA";

    let registroLinea = form;
    registroLinea.fechaRegistro = new Date();
    registroLinea.importeDeposito = +form.importeDeposito;
    registroLinea.estado = this.estadoRegistroLineaEnum.EMITIDO;
    registroLinea.cliente = cliente;
    registroLinea.registroLineaDetalle = this.dataSource.data;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Registrando...");

        this.recaudaService.createRegistroLinea(registroLinea).subscribe(
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
        location.reload();
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
    this.messageService.msgError(MESSAGES.API.ERROR_SERVICE, () => { });
  }

  //Mensaje de validaciónes
  maxLength(max: number) {
    return "No escribas más de " + max + " caracteres.";
  }

  minLength(min: number) {
    return "No escribas menos de " + min + " caracteres.";
  }

  // Configuration

  private parseRoute(node: ActivatedRouteSnapshot) {
    if (node.data["breadcrumb"]) {
      if (node.url.length) {
        let urlSegments: UrlSegment[] = [];
        node.pathFromRoot.forEach((routerState) => {
          urlSegments = urlSegments.concat(routerState.url);
        });
        let url = urlSegments
          .map((urlSegment) => {
            return urlSegment.path;
          })
          .join("/");
        this.breadcrumbs.push({
          name: node.data["breadcrumb"],
          url: "/" + url,
        });
      }
    }
    if (node.firstChild) {
      this.parseRoute(node.firstChild);
    }
  }

  public onPsScrollY(event) {
    event.target.scrollTop > 300
      ? (this.backToTop.nativeElement.style.display = "flex")
      : (this.backToTop.nativeElement.style.display = "none");
  }

  public scrollToTop() {
    this.pss.forEach((ps) => {
      if (
        ps.elementRef.nativeElement.id == "main" ||
        ps.elementRef.nativeElement.id == "main-content"
      ) {
        ps.scrollToTop(0, 250);
      }
    });
  }

  @HostListener("window:resize")
  public onWindowResize(): void {
    if (window.innerWidth <= 768) {
    } else {
    }
  }
}
