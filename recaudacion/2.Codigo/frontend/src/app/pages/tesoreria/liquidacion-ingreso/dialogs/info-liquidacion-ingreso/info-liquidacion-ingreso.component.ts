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
import { FormGroup, FormBuilder } from "@angular/forms";
import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { Liquidacion } from "src/app/core/models/liquidacion";
import { LiquidacionService } from "src/app/core/services/liquidacion.service";
import { FuenteFinanciamiento } from "src/app/core/models/fuentefinanciamiento";
import { TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { ClienteService } from "src/app/core/services/cliente.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { MessageService } from "src/app/core/services/message.service";
import { UnidadEjecutoraService } from "src/app/core/services/unidad-ejecutora.service";
import { EditLiquidacionIngresoComponent } from "../edit-liquidacion-ingreso/edit-liquidacion-ingreso.component";

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
  selector: 'app-info-liquidacion-ingreso',
  templateUrl: './info-liquidacion-ingreso.component.html',
  styleUrls: ['./info-liquidacion-ingreso.component.scss'],
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
export class InfoLiquidacionIngresoComponent implements OnInit {
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
  ) {
    this.form = this.fb.group({
      numero: [data.numero],
      fechaRegistro: [data.fechaRegistro],
      procedencia: [data.procedencia],
      cuentaCorriente: [data.cuentaCorrienteId],
      fuenteFinanciamientoId: [data?.fuenteFinanciamientoId],
      rubro: [null]
    });
  }

  ngOnInit() {
    this.onLoadData();
    this.onLoadMaestras();

  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
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
            element.numeroDenominacion =element?.numero + " - " + element?.denominacion;
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
