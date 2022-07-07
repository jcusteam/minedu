import { Component, OnInit, Inject } from "@angular/core";
import { DatePipe } from "@angular/common";
import {
  MAT_DATE_LOCALE,
  DateAdapter,
  MAT_DATE_FORMATS,
} from "@angular/material/core";
import {
  MomentDateAdapter,
  MAT_MOMENT_DATE_ADAPTER_OPTIONS,
} from "@angular/material-moment-adapter";
import { MatTableDataSource } from "@angular/material/table";
import { FormGroup, FormBuilder } from "@angular/forms";
import { TipoReciboIngreso } from "src/app/core/models/tiporeciboingreso";
import { RegistroLineaService } from "src/app/core/services/registro-linea.service";
import { TipoReciboIngresoService } from "src/app/core/services/tipo-recibo-ingreso.service";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RegistroLinea, RegistroLineaDetalle } from "src/app/core/models/registrolinea";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { EditRegistroLineaComponent } from "../edit-registro-linea/edit-registro-linea.component";
import { EstadoRegistroLineaEnum, TipoDocEnum, TipoDocIdentidadEnum, TipoReciboIngresoEnum, ValidaDepositoEnum } from "src/app/core/enums/recaudacion.enum";


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
  selector: "app-info-registro-linea",
  templateUrl: "./info-registro-linea.component.html",
  styleUrls: ["./info-registro-linea.component.scss"],
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
export class InfoRegistroLineaComponent implements OnInit {
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
    public dialogRef: MatDialogRef<EditRegistroLineaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RegistroLinea,
    private cuentaCorrienteService: CuentaCorrienteService,
    private registroLineaService: RegistroLineaService,
    private tipoReciboIngresoService: TipoReciboIngresoService,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private fb: FormBuilder) {
    this.form = this.fb.group({
      registroLineaId: [data?.registroLineaId],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      cuentaCorrienteId: [data?.cuentaCorrienteId],
      bancoId: [data?.bancoId],
      clienteId: [data?.clienteId],
      tipoDocumentoId: [data?.tipoDocumentoId],
      numero: [data?.numero],
      fechaRegistro: [data?.fechaRegistro],
      tipoReciboIngresoId: [data?.tipoReciboIngresoId],
      numeroDeposito: [data?.numeroDeposito],
      importeDeposito: [data?.importeDeposito],
      fechaDeposito: [data?.fechaDeposito],
      numeroOficio: [data?.numeroOficio],
      numeroComprobantePago: [data?.numeroComprobantePago],
      expedienteSiaf: [data?.expedienteSiaf],
      numeroResolucion: [data?.numeroResolucion],
      expedienteESinad: [data?.expedienteESinad],
      validarDeposito: [data?.validarDeposito],
      observacion: [data?.observacion],
      estado: [data?.estado],
      usuarioModificador: [data?.usuarioModificador],

      // Cliente
      tipoDocumentoIdentidadId: [data?.cliente?.tipoDocumentoIdentidadId],
      numeroDocumento: [data?.cliente?.numeroDocumento?.trim()],
      clienteNombre: [data?.cliente?.nombre?.trim()],
      correo: [data?.cliente?.correo?.trim()],
      // Extra
      cuentaCorriente: [data?.cuentaCorriente],
      // Detalle
      clasificadorIngreso: [null],
      importe: [0],
    });
  }

  ngOnInit() {
    this.onloadData();
    this.onLoadMaestras();
    if (this.data.tipoDocumentoId == this.tipoReciboIngresoEnum.DEPOSITO_INDEBIDO) {
      this.hiddenExpedienteESinad = true;
    }
  }

  onloadData() {
    this.registroLineaService.getRegistroLineaById(this.data.registroLineaId).subscribe(
      response => {
        if (response.success) {
          this.dataSource.data = response.data.registroLineaDetalle;
          this.dataSource._updateChangeSubscription();
        }
      }
    )
  }

  getTotalImporte() {
    return this.dataSource.data.map((t) => t.importe).reduce((acc, value) => acc + value, 0);
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
            obj.tipoReciboIngresoId == 2 ||
            obj.tipoReciboIngresoId == 7 ||
            obj.tipoReciboIngresoId == 8
        );
      });

    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.numeroDenominacion =element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();

          let cuentas = this.cuentasCorrientes.filter(x => x.cuentaCorrienteId == this.data.cuentaCorrienteId)
          if (cuentas.length > 0) {
            this.form.patchValue({ cuentaCorriente: cuentas[0] });
          }
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

  onSubmit() {
  }

}
