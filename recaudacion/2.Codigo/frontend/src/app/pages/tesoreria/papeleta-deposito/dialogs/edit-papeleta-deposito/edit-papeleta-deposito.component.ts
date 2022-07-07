import {
  Component,
  OnInit,
  Inject,
  DEFAULT_CURRENCY_CODE,
} from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { DatePipe } from "@angular/common";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import {
  DateAdapter,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from "@angular/material/core";
import {
  MAT_MOMENT_DATE_ADAPTER_OPTIONS,
  MomentDateAdapter,
} from "@angular/material-moment-adapter";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { PapeletaDeposito, PapeletaDepositoDetalle } from "src/app/core/models/PapeletaDeposito";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { PapeletaDepositoService } from "src/app/core/services/papeleta-deposito.service";
import { ReciboIngresoService } from "src/app/core/services/recibo-ingreso.service";
import { ReciboIngreso } from "src/app/core/models/reciboingreso";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
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
  selector: "app-edit-papeleta-deposito",
  templateUrl: "./edit-papeleta-deposito.component.html",
  styleUrls: ["./edit-papeleta-deposito.component.scss"],
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
export class EditPapeletaDepositoComponent implements OnInit {
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  form: FormGroup;
  papeletaDepositoDetalles: PapeletaDepositoDetalle[] = [];

  dataSource = new MatTableDataSource(this.papeletaDepositoDetalles);
  displayedColumns: string[] = [
    "index",
    "numero",
    "fechaEmision",
    "cuentaCorriente",
    "tipoCaptacion",
    "numeroCheque",
    "importe",
    "actions",
  ];

  constructor(
    public dialogRef: MatDialogRef<EditPapeletaDepositoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PapeletaDeposito,
    private fb: FormBuilder,
    private cuentaCorrienteService: CuentaCorrienteService,
    private papeletaDepositoService: PapeletaDepositoService,
    private reciboIngesoService: ReciboIngresoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      papeletaDepositoId: [data?.papeletaDepositoId],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      bancoId: [data?.bancoId],
      cuentaCorrienteId: [data?.cuentaCorrienteId],
      tipoDocumentoId: [data?.tipoDocumentoId],
      numero: [data?.numero],
      fecha: [data?.fecha],
      monto: [data?.monto,
      Validators.compose([
        Validators.required,
        Validators.pattern(/^[.\d]+$/),
        Validators.maxLength(12)
      ])
      ],
      descripcion: [data?.descripcion,
      Validators.compose([
        Validators.required,
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
        Validators.maxLength(300)
      ])
      ],
      estado: [data?.estado],
      usuarioModificador: [data?.usuarioModificador],
      // Extra
      cuentaCorriente: [data?.cuentaCorriente, Validators.compose([Validators.required])],
      reciboIngreso: [null],
      reciboIngresoNumero: [null,
        Validators.compose([
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(6)
        ])
      ],
      reciboIngresoFechaEmision: [null],
      reciboIngresoImporte: [null],
      reciboIngresoCuentaCorriente: [null],
      tipoCaptacionId: [null],
      numeroCheque: [null],
    });
  }

  ngOnInit() {
    this.loadMaestras();
    this.loadData();
  }

  loadData() {
    this.papeletaDepositoService.getPapeletaDepositoById(this.data.papeletaDepositoId).subscribe(
      response => {
        if (response.success) {
          this.dataSource.data = response.data.papeletaDepositoDetalle;
          this.dataSource._updateChangeSubscription();
        }
      }
    );
  }

  loadMaestras() {
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
  }

  onSearchReciboIngreso() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.get("cuentaCorriente").valid || !this.form.get("reciboIngresoNumero").valid) {
      return;
    }

    const unidadEjecutoraId = this.form.get("unidadEjecutoraId").value;
    const cuentaCorrienteId = this.form.get("cuentaCorrienteId").value;
    const numero = this.form.get("reciboIngresoNumero").value;

    if (isNaN(numero)) {
      return;
    }

    this.form.patchValue({
      reciboIngreso: null,
      reciboIngresoFechaEmision: null,
      reciboIngresoImporte: null,
      reciboIngresoCuentaCorriente: null,
      tipoCaptacionId: null,
      numeroCheque: null,
    });

    this.messageService.msgLoad("Buscando...");
    this.papeletaDepositoService.getReciboIngresoByNroEjecutora(numero, unidadEjecutoraId, cuentaCorrienteId)
      .subscribe(
        (response) => {
          var message = response.messages.join(" , ");
          if (response.success) {
            let reciboIngreso = response.data;
            this.messageService.msgAutoClose();
            this.form.patchValue({
              reciboIngreso: reciboIngreso,
              reciboIngresoFechaEmision: reciboIngreso?.fechaEmision,
              reciboIngresoImporte: reciboIngreso?.importeTotal,
              reciboIngresoCuentaCorriente: reciboIngreso?.cuentaCorriente,
              tipoCaptacionId: reciboIngreso?.tipoCaptacionId,
              numeroCheque: reciboIngreso?.numeroCheque,
            });
          } else {
            if (response.messageType == TYPE_MESSAGE.INFO) {
              this.messageService.msgInfo(message, () => { });
            }
            else if (response.messageType == TYPE_MESSAGE.WARNING) {
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

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {
    this.form.patchValue({ bancoId: cuentaCorriente.bancoId, cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId })
  }

  addRowData() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.get("cuentaCorriente").valid || !this.form.get("reciboIngreso").valid) {
      return;
    }

    let reciboIngreso: ReciboIngreso = this.form.get("reciboIngreso").value;

    if(reciboIngreso == null){
      return;
    }
    
    let detalle = new PapeletaDepositoDetalle();

    let detalles = this.dataSource.data.filter(x => x.reciboIngresoId == reciboIngreso.reciboIngresoId);

    if (detalles.length > 0) {
      this.messageService.msgWarning("El recibo de ingreso ya se encuentra agregado", () => {
      });
      return;
    }

    detalle.reciboIngreso = reciboIngreso;
    detalle.reciboIngresoId = reciboIngreso.reciboIngresoId;
    detalle.monto = reciboIngreso.importeTotal;
    detalle.estado = "1";
    detalle.usuarioCreador = this.data.usuarioCreador;
    this.dataSource.data.push(detalle);
    this.dataSource._updateChangeSubscription();

    this.form.patchValue({
      reciboIngresoNumero: null,
      reciboIngreso: null,
      reciboIngresoFechaEmision: null,
      reciboIngresoImporte: null,
      reciboIngresoCuentaCorriente: null,
      tipoCaptacionId: null,
      numeroCheque: null,
    });

    this.form.patchValue({ monto: this.getTotal() });
  }

  deleteRowData(data: PapeletaDepositoDetalle) {
    this.dataSource.data = this.dataSource.data.filter((obj) => obj !== data);

    this.form.patchValue({ monto: this.getTotal() });
  }

  getTotal() {
    return this.dataSource.data.map((t) => t.monto).reduce((acc, value) => acc + value, 0);
  }

  onSubmit(form: PapeletaDeposito) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle de la papeleta de depósito", () => { });
      return;
    }

    let papeletaDeposito = form;
    papeletaDeposito.monto = this.getTotal();
    papeletaDeposito.papeletaDepositoDetalle = this.dataSource.data;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.papeletaDepositoService.updatePapeletaDeposito(papeletaDeposito).subscribe(
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
