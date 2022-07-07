import { Component, OnInit, Inject, DEFAULT_CURRENCY_CODE } from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { DatePipe } from "@angular/common";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, } from "@angular/material/core";
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from "@angular/material-moment-adapter";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";

import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { TipoReciboIngreso } from "src/app/core/models/tiporeciboingreso";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import { RegistroLinea, RegistroLineaDetalle } from "src/app/core/models/registrolinea";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { TipoReciboIngresoService } from "src/app/core/services/tipo-recibo-ingreso.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { DepositoBancoService } from "src/app/core/services/deposito-banco.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { RegistroLineaService } from "src/app/core/services/registro-linea.service";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { DepositoBancoDetalle } from "src/app/core/models/depositobanco";
import { EstadoRegistroLineaEnum, TipoDocEnum, TipoDocIdentidadEnum, TipoReciboIngresoEnum, ValidaDepositoEnum } from "src/app/core/enums/recaudacion.enum";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { PideService } from 'src/app/core/services/pide.service';
import { Cliente } from "src/app/core/models/cliente";

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
  selector: "app-new-registro-linea",
  templateUrl: "./new-registro-linea.component.html",
  styleUrls: ["./new-registro-linea.component.scss"],
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
export class NewRegistroLineaComponent implements OnInit {

  //Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoRegLineaEnum = EstadoRegistroLineaEnum;
  validaDepositoEnum = ValidaDepositoEnum;
  tipoReciboIngresoEnum = TipoReciboIngresoEnum;

  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  clasificadorIngresos: ClasificadorIngreso[] = [];
  filteredClasificadorIngresos;

  tipoReciboIngresos: TipoReciboIngreso[] = [];

  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];

  displayedColumns: string[] = [
    "index",
    "clasificadorIngreso",
    "importe",
    "actions",
  ];

  registroLineaDetalles: RegistroLineaDetalle[] = [];
  dataSource = new MatTableDataSource(this.registroLineaDetalles);
  hiddenExpedienteESinad = false;
  startDate = new Date();
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewRegistroLineaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RegistroLinea,
    private cuentaCorrienteService: CuentaCorrienteService,
    private registroLineaService: RegistroLineaService,
    private tipoReciboIngresoService: TipoReciboIngresoService,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private depositoBancoService: DepositoBancoService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private pideService: PideService,
    private fb: FormBuilder,
    private datePipe: DatePipe,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      registroLineaId: [0],
      unidadEjecutoraId: [data?.unidadEjecutoraId, Validators.compose([Validators.required])],
      cuentaCorrienteId: [0, Validators.compose([Validators.required])],
      bancoId: [null, Validators.compose([Validators.required])],
      clienteId: [0],
      tipoDocumentoId: [data?.tipoDocumentoId, Validators.compose([Validators.required])],
      numero: ["0000"],
      fechaRegistro: [new Date()],
      tipoReciboIngresoId: [null, Validators.compose([Validators.required])],
      depositoBancoDetalleId: [null],
      numeroDeposito: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-z0-9]+$'),
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
      numeroESinad: [null],
      observacion: ["",
        Validators.compose([
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
          Validators.maxLength(300)
        ])
      ],
      estado: [data.estado, Validators.compose([Validators.required])],
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
    this.onLoadMaestras();
  }

  onLoadMaestras() {

    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService
      .getTipoDocumentoIdentidades()
      .subscribe((response) => {
        if (response.success) {
          this.tipoDocumentoClientes = response.data;
        }
      });

    // Tipo Recibo  Ingresos
    this.tipoReciboIngresoService
      .getTipoReciboIngresos()
      .subscribe((response) => {
        this.tipoReciboIngresos = response.data.filter(
          (obj) =>
            obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.EJECUCION_CARTA_FIANAZA ||
            obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.DEVOLUCION_SALGO_VIATICO ||
            obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.DEPOSITO_INDEBIDO
        );
      });

    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.numeroDenominacion = element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();
        }
      });

    this.clasificadorIngresoService
      .getClasificadorIngresos()
      .subscribe((response) => {
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

  selectionTipoDocIdentidad(id: number) {
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

  // Limpiar formulario
  clearForm() {
    this.form.patchValue({
      clienteNombre: null,
      correo: null,
      direccion: null
    });
  }

  // Key Up Nro Doc
  onKeyUpNroDoc(nro) {
    this.clearForm();
  }

  searchNroDoc() {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.clearForm();

    if (!this.form.get("tipoDocumentoIdentidadId").valid) {
      return;
    }

    if (!this.form.get("numeroDocumento").valid) {
      return;
    }

    const numeroDocumento = this.form.get("numeroDocumento").value;
    const tipoDocumentoIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;

    if (tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI) {
      this.getReniecData(numeroDocumento);

    } else if (tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE) {
      this.getMigracionData(numeroDocumento);

    } else if (tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.RUC) {
      this.getSunatData(numeroDocumento);
    }
  }

  // Consulta SUNAT
  getReniecData(numeroDoc) {
    this.messageService.msgLoad("Consultado RENIEC...");
    this.pideService.getReniecByDni(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          var nombre = response.data.nombreCompleto;
          var direccion = response.data.domicilioApp;
          this.form.patchValue({
            clienteNombre: nombre,
            direccion: direccion
          });
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
        if (response.success) {
          var nombre = response.data.strNombreCompleto;
          var direccion = "";
          this.form.patchValue({
            clienteNombre: nombre,
            direccion: direccion
          });
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

  // Consulta SUNAT
  getSunatData(numeroDoc) {
    this.messageService.msgLoad("Consultado SUNAT...");
    this.pideService.getSunatByRuc(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          var nombre = response.data.ddp_nombre;
          var direccion = response.data.desc_domi_fiscal;
          this.form.patchValue({
            clienteNombre: nombre,
            direccion: direccion
          });
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

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {
    this.form.patchValue({
      cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId,
      bancoId: cuentaCorriente.bancoId
    });
  }

  // Clasificador Ingreso
  seletedClasificadorIngresoOption(clasificadorIngreso: ClasificadorIngreso) {
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

  addRowData() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

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
    registroLineaDetalle.usuarioCreador = this.data.usuarioCreador;
    this.dataSource.data.push(registroLineaDetalle);
    this.dataSource._updateChangeSubscription();

    this.form.patchValue({
      clasificadorIngreso: null,
      importe: 0.0,
    });
  }

  deleteRowData(data: RegistroLineaDetalle) {
    this.dataSource.data = this.dataSource.data.filter((obj) => obj !== data);

  }

  getTotalImporte() {
    return this.dataSource.data.map((t) => t.importe).reduce((acc, value) => acc + value, 0);
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

    this.data.validarDeposito = "0";
    const numeroDeposito = this.form.get("numeroDeposito").value;
    const fecha = this.form.get("fechaDeposito").value;
    let cuentaCorriente: CuentaCorriente = this.form.get("cuentaCorriente").value;
    const fechaDeposito = this.datePipe.transform(fecha, "yyyy-MM-dd");
    const cuentaCorrienteId = cuentaCorriente.cuentaCorrienteId;

    this.depositoBancoService
      .getDepositoBancoDetalleByNumeroFecha(
        numeroDeposito,
        fechaDeposito,
        cuentaCorrienteId
      )
      .subscribe((response) => {
        var message = response.messages.join(",");
        if (response.success) {
          this.onOpenAlertDepositoBanco("", "", response.data);
        } else {
          if (response.messageType == "info") {
            this.messageService.msgInfo(message, () => { });
          } else if (response.messageType == "warning") {
            this.messageService.msgWarning(message, () => { });
          } else {
            this.messageService.msgError(message, () => { });
          }
        }
      });
  }

  onOpenAlertDepositoBanco(messageTitle, messageIcon, depositoBancoDetalle: DepositoBancoDetalle) {

    if (depositoBancoDetalle.estado == "1") {
      messageTitle = "Correcto!";
      messageIcon = "success";
      this.data.validarDeposito = "1";
    } else {
      messageTitle = "Utilizado!";
      messageIcon = "warning";
      this.data.validarDeposito = "0";
    }

    let cuentaCorriente: CuentaCorriente = this.form.get("cuentaCorriente").value;
    var fechaDeposito = this.datePipe.transform(depositoBancoDetalle.fechaDeposito, "dd-MM-yyyy");
    let deposito = {
      numeroCuenta: cuentaCorriente?.numero,
      numeroDeposito: depositoBancoDetalle?.numeroDeposito,
      importeDep: depositoBancoDetalle?.importe,
      fechaDeposito: fechaDeposito
    }

    this.messageService.msgLgValidDeposito(messageTitle, messageIcon, deposito, () => { });
  }

  onSubmit(form: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => {
      });
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle del registro en línea.", () => {
      });
      return;
    }

    if (this.getTotalImporte() != (+form.importeDeposito)) {
      let message = "El importe total del detalle no es igual al importe del depósito";
      this.messageService.msgWarning(message, () => { });
      return;
    }
    
    let cliente = new Cliente();
    cliente.tipoDocumentoIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;
    cliente.numeroDocumento = this.form.get("numeroDocumento").value;
    cliente.nombre = this.form.get("clienteNombre").value;
    cliente.correo = this.form.get("correo").value;
    cliente.direccion = "";

    let registroLinea = form;
    registroLinea.fechaRegistro = new Date();
    registroLinea.importeDeposito = +form.importeDeposito;
    registroLinea.cliente = cliente;
    registroLinea.registroLineaDetalle = this.dataSource.data;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");

        this.registroLineaService.createRegistroLinea(registroLinea).subscribe(
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

}
