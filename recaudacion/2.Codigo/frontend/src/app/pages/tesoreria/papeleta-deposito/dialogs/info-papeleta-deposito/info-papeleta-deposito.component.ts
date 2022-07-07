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
  MAT_MOMENT_DATE_FORMATS,
  MomentDateAdapter,
} from "@angular/material-moment-adapter";
import { FormGroup, FormBuilder } from "@angular/forms";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { PapeletaDeposito, PapeletaDepositoDetalle } from "src/app/core/models/PapeletaDeposito";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { PapeletaDepositoService } from "src/app/core/services/papeleta-deposito.service";
import { MessageService } from "src/app/core/services/message.service";

@Component({
  selector: 'app-info-papeleta-deposito',
  templateUrl: './info-papeleta-deposito.component.html',
  styleUrls: ['./info-papeleta-deposito.component.scss'],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    { provide: DEFAULT_CURRENCY_CODE, useValue: "PEN" },
  ],
})
export class InfoPapeletaDepositoComponent implements OnInit {
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
    public dialogRef: MatDialogRef<InfoPapeletaDepositoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PapeletaDeposito,
    private fb: FormBuilder,
    private cuentaCorrienteService: CuentaCorrienteService,
    private papeletaDepositoService: PapeletaDepositoService,
    private messageService: MessageService) {
    this.form = this.fb.group({
      numero: [data?.numero],
      fecha: [data?.fecha],
      bancoId: [data?.bancoId],
      cuentaCorriente: [data?.cuentaCorriente],
      monto: [data.monto],
      descripcion: [data.descripcion],
      estado: [data.estado],
      reciboIngreso: [null],
      reciboIngresoNumero: [null],
      reciboIngresoFechaEmision: [null],
      reciboIngresoImporte: [null],
      reciboIngresoCuentaCorriente: [null],
      tipoCaptacionId: [null],
      numeroCheque: [null],
    });
  }

  ngOnInit() {

    this.loadData();
    this.loadMaestras();
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

  getTotal() {
    return this.dataSource.data.map((t) => t.monto).reduce((acc, value) => acc + value, 0);
  }

  onSubmit() {

  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onNoClick(): void {
    this.dialogRef.close(0);
  }
}
