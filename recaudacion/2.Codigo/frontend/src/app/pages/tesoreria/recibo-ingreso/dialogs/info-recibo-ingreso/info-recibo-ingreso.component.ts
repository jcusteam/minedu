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
} from "@angular/forms";
import {
  ReciboIngreso,
  ReciboIngresoDetalle,
} from "src/app/core/models/reciboingreso";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { ReciboIngresoService } from "src/app/core/services/recibo-ingreso.service";
import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { TipoReciboIngresoService } from "src/app/core/services/tipo-recibo-ingreso.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
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
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { TipoCaptacionService } from "src/app/core/services/tipo-captacion.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { DepositoBancoDetalle } from "src/app/core/models/depositobanco";
import { TipoCaptacion } from "src/app/core/models/tipocaptacion";

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
  selector: "app-info-recibo-ingreso",
  templateUrl: "./info-recibo-ingreso.component.html",
  styleUrls: ["./info-recibo-ingreso.component.scss"],
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
export class InfoReciboIngresoComponent implements OnInit {
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  tipoReciboIngresos: TipoReciboIngreso[] = [];

  fuenteFinanciamientos: FuenteFinanciamiento[] = [];

  clasificadorIngresos: ClasificadorIngreso[] = [];
  filteredClasificadorIngresos;
  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];

  tipoCaptaciones: TipoCaptacion[] = [];
  displayedColumns: string[] = [
    "index",
    "clasificadorIngreso",
    "importe"
  ];

  reciboIngresoDetalles: ReciboIngresoDetalle[] = [];
  dataSource = new MatTableDataSource(this.reciboIngresoDetalles);

  hidenDeposito = false;
  hidenCheque = false;
  depositoIsValid = false;
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoReciboIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ReciboIngreso,
    private cuentaCorrienteService: CuentaCorrienteService,
    private reciboIngresoService: ReciboIngresoService,
    private fuenteFinanciamientoService: FuenteFinanciamientoService,
    private tipoReciboIngresoServie: TipoReciboIngresoService,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private tipoCaptacionService: TipoCaptacionService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private fb: FormBuilder) {
    this.form = this.fb.group({
      numero: [data.numero],
      fechaEmision: [data.fechaEmision],
      tipoReciboIngresoId: [
        data.tipoReciboIngresoId
      ],
      cuentaCorriente: [
        data.cuentaCorriente
      ],
      fuenteFinanciamientoId: [
        data.fuenteFinanciamientoId
      ],
      tipoCaptacionId: [
        data.tipoCaptacionId
      ],
      importeTotal: [
        data.importeTotal
      ],
      numeroDeposito: [
        data.numeroDeposito?.trim()
      ],
      fechaDeposito: [data.fechaDeposito],
      numeroCheque: [
        data.numeroCheque?.trim()
      ],
      numeroOficio: [
        data.numeroOficio?.trim()
      ],
      numeroComprobantePago: [
        data.numeroComprobantePago?.trim()
      ],
      expedienteSiaf: [
        data.expedienteSiaf?.trim()
      ],
      numeroResolucion: [
        data.numeroResolucion?.trim()
      ],
      cartaOrden: [data.cartaOrden],
      liquidacionIngreso: [
        data.liquidacionIngreso?.trim()
      ],
      papeletaDeposito: [
        data.papeletaDeposito?.trim()
      ],
      concepto: [data.concepto],
      referencia: [data.referencia],
      clasificadorIngreso: [null],
      importe: [null],
      tipoDocumentoIdentidadId: [
        data.cliente?.tipoDocumentoIdentidadId,
      ],
      numeroDocumento: [
        data.cliente?.numeroDocumento?.trim()
      ],
      nombreCliente: [
        data.cliente?.nombre,
      ],
    });
  }

  ngOnInit() {
    if (this.data.tipoCaptacionId == 2) {
      this.hidenDeposito = true;
    }

    if (this.data.tipoCaptacionId == 3) {
      this.hidenCheque = true;
    }

    this.onLoadData();
    this.onLoadMaestras();
  }

  onLoadData() {
    this.reciboIngresoService.getReciboIngresoById(this.data.reciboIngresoId).subscribe(
      response => {
        if (response.success) {
          this.dataSource.data = response.data.reciboIngresoDetalle;
          this.dataSource._updateChangeSubscription();
        }
      }
    );
  }

  getTotal(){
    return this.dataSource.data.map((t) => t.importe).reduce((acc, value) => acc + value, 0);
  }

  onLoadMaestras() {

    // Tipo Recibo  Ingresos
    this.tipoReciboIngresoServie
      .getTipoReciboIngresos()
      .subscribe((response) => {
        if (response.success) {
          this.tipoReciboIngresos = response.data;
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
        this.tipoCaptaciones = response.data;
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
            element.descripcion =
              element?.codigo + " - " + element?.descripcion;
          });
          this.clasificadorIngresos = response.data;
          this.filteredClasificadorIngresos = this.clasificadorIngresos.slice();
        }
      });
  }

  onSubmit() {

  }

}
