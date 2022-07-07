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
import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { Liquidacion } from "src/app/core/models/liquidacion";
import { LiquidacionService } from "src/app/core/services/liquidacion.service";
import { FuenteFinanciamiento } from "src/app/core/models/fuentefinanciamiento";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
import { ClienteService } from "src/app/core/services/cliente.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { UnidadEjecutoraService } from "src/app/core/services/unidad-ejecutora.service";
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
  selector: "app-edit-liquidacion-ingreso",
  templateUrl: "./edit-liquidacion-ingreso.component.html",
  styleUrls: ["./edit-liquidacion-ingreso.component.scss"],
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
export class EditLiquidacionIngresoComponent implements OnInit {
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  form: FormGroup;
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;
  fuenteFinanciamientos: FuenteFinanciamiento[] = [];

  liquidacionDetalles: any[] = [];
  dataSource = new MatTableDataSource(this.liquidacionDetalles);
  displayedColumns: string[] = [
    "codigo",
    "descripcion",
    "tipoCaptacion",
    "importeParcial"
  ];

  spans = [];

  constructor(
    public dialogRef: MatDialogRef<EditLiquidacionIngresoComponent>,
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
      numero: [data.numero],
      fechaRegistro: [data.fechaRegistro],
      procedencia: [data.procedencia, Validators.compose([Validators.required])],
      cuentaCorriente: [null, Validators.compose([Validators.required])],
      fuenteFinanciamientoId: [data?.fuenteFinanciamientoId, Validators.compose([Validators.required])],
      rubro: [null]
    });
  }

  ngOnInit() {
    console.log(this.data);
    this.onLoadData();
    this.onLoadMaestras();

  }

  onLoadData() {
    this.liquidacionService.getLiquidacionById(this.data.liquidacionId).subscribe(
      (response) => {
        if (response.success) {
          let fuente = this.data?.fuenteFinanciamiento;
          if (fuente) {
            let rubro = fuente?.rubroCodigo + " - " + fuente?.rubroDescripcion;
            this.form.patchValue({
              rubro: rubro
            });
          }
          this.dataSource.data = response.data.liquidacionDetalle;
          this.dataSource._updateChangeSubscription();
          this.cacheSpan('codigo', d => d.clasificadorIngreso.codigo);
          this.cacheSpan('descripcion', d => d.clasificadorIngreso.descripcion);
        }
      }
    );

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
              this.data.clienteId = response.data.clienteId;
              this.form.patchValue({ procedencia: response.data.nombre })
            }
          );
        }
      }
    );

    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.numeroDenominacion = element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();

          let cuentaCorriente = this.cuentasCorrientes.filter(x => x.cuentaCorrienteId == this.data.cuentaCorrienteId)[0];
          if (cuentaCorriente) {
            this.form.patchValue({ cuentaCorriente: cuentaCorriente })
          }
        }
      });
  }


  onSubmit(form: Liquidacion) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let liquidacion = form;
    liquidacion.liquidacionId = this.data.liquidacionId;
    liquidacion.unidadEjecutoraId = this.data.unidadEjecutoraId;
    liquidacion.tipoDocumentoId = this.data.tipoDocumentoId;
    liquidacion.fuenteFinanciamientoId = this.data.fuenteFinanciamientoId;
    liquidacion.clienteId = this.data.clienteId;
    liquidacion.cuentaCorrienteId = this.data.cuentaCorrienteId;
    liquidacion.total = this.data.total;
    liquidacion.estado = this.data.estado;
    liquidacion.usuarioModificador = this.data.usuarioModificador;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.liquidacionService.updateLiquidacion(liquidacion).subscribe(
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

  cacheSpan(key, accessor) {
    for (let i = 0; i < this.dataSource.data.length;) {
      let currentValue = accessor(this.dataSource.data[i]);
      let count = 1;

      // Iterate through the remaining rows to see how many match
      // the current value as retrieved through the accessor.
      for (let j = i + 1; j < this.dataSource.data.length; j++) {
        if (currentValue != accessor(this.dataSource.data[j])) {
          break;
        }

        count++;
      }

      if (!this.spans[i]) {
        this.spans[i] = {};
      }

      // Store the number of similar values that were found (the span)
      // and skip i to the next unique row.
      this.spans[i][key] = count;
      i += count;
    }
  }

  getRowSpan(col, index) {
    return this.spans[index] && this.spans[index][col];
  }

}
