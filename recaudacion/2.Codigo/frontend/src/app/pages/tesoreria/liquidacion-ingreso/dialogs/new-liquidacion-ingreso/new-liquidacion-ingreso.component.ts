import { Component, OnInit, Inject, DEFAULT_CURRENCY_CODE, } from "@angular/core";
import { DatePipe } from "@angular/common";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, } from "@angular/material/core";
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter, } from "@angular/material-moment-adapter";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";

import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { Liquidacion } from "src/app/core/models/liquidacion";
import { LiquidacionService } from "src/app/core/services/liquidacion.service";
import { FuenteFinanciamiento } from "src/app/core/models/fuentefinanciamiento";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { UnidadEjecutoraService } from "src/app/core/services/unidad-ejecutora.service";
import { ClienteService } from "src/app/core/services/cliente.service";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
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
  selector: 'app-new-liquidacion-ingreso',
  templateUrl: './new-liquidacion-ingreso.component.html',
  styleUrls: ['./new-liquidacion-ingreso.component.scss'],
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
export class NewLiquidacionIngresoComponent implements OnInit {

  // Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;
  fuenteFinanciamientos: FuenteFinanciamiento[] = [];
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewLiquidacionIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Liquidacion,
    private fb: FormBuilder,
    private liquidacionService: LiquidacionService,
    private fuenteFinanciamientoService: FuenteFinanciamientoService,
    private cuentaCorrienteService: CuentaCorrienteService,
    private unidadEjecutoraService: UnidadEjecutoraService,
    private clienteService: ClienteService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      liquidacionId: [0],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      tipoDocumentoId: [data?.tipoDocumentoId],
      fuenteFinanciamientoId: [0],
      clienteId: [0],
      reciboIngresoId: [null],
      cuentaCorrienteId: [0],
      numero: ["000"],
      procedencia: [null, Validators.compose([Validators.required])],
      fechaRegistro: [new Date()],
      total: [0],
      estado: [data?.estado],
      usuarioCreador: [data?.usuarioCreador],
      //Extra
      cuentaCorriente: [null, Validators.compose([Validators.required])],
      fuenteFinanciamiento: [null, Validators.compose([Validators.required])],
      rubro: [null]
    });
  }

  ngOnInit() {
    this.onLoadMaestras();
  }

  onLoadMaestras() {

    this.fuenteFinanciamientoService.getFuenteFinanciamientos().subscribe(
      (response) => {
        if (response.success) {
          this.fuenteFinanciamientos = response.data;
        }
      }
    );

    //Procedencia
    this.unidadEjecutoraService.getUnidadEjecutoraById(this.data.unidadEjecutoraId).subscribe(
      (response) => {
        if (response.success) {
          this.clienteService.getClienteByTipoNroDocumento(this.tipoDocIdentidadEnum.RUC, response.data.numeroRuc).subscribe(
            (response) => {
              this.form.patchValue({ clienteId: response.data?.clienteId, procedencia: response.data?.nombre })
            }
          );
        }
      }
    );

    // Cuentas Corrientes
    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.numeroDenominacion =element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();
        }
      });
  }

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente: CuentaCorriente) {
    this.form.patchValue({ cuentaCorrienteId: cuentaCorriente.cuentaCorrienteId });
  }

  selectedChangeFuenteFinanciamiento(fuenteFinanciamiento: FuenteFinanciamiento) {
    this.form.patchValue({
      fuenteFinanciamientoId: fuenteFinanciamiento.fuenteFinanciamientoId,
      rubro: fuenteFinanciamiento?.rubroCodigo.trim() + " - " + fuenteFinanciamiento?.rubroDescripcion
    });
  }

  onSubmit(form: Liquidacion) {


    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
    }

    let liquidacion = form;
    liquidacion.total = +form.total;
    liquidacion.fechaRegistro = new Date();
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.liquidacionService.createLiquidacion(liquidacion).subscribe(
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
