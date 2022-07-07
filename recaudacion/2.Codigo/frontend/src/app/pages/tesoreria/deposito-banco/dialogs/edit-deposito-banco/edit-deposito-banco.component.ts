import { Component, OnInit, Inject, DEFAULT_CURRENCY_CODE } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { FormBuilder, Validators, FormGroup } from "@angular/forms";
import { DatePipe } from "@angular/common";
import { MatTableDataSource } from "@angular/material/table";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from "@angular/material/core";

import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { DepositoBancoService } from "src/app/core/services/deposito-banco.service";
import {DepositoBanco,DepositoBancoDetalle,} from "src/app/core/models/depositobanco";

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
  selector: "app-edit-deposito-banco",
  templateUrl: "./edit-deposito-banco.component.html",
  styleUrls: ["./edit-deposito-banco.component.scss"],
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
export class EditDepositoBancoComponent implements OnInit {

  depositoDetalle: DepositoBancoDetalle[] = [];
  displayedColumns: string[] = [
    "index",
    "numeroDeposito",
    "tipoDocumentoIdentidad",
    "numeroDocCliente",
    "nombreCliente",
    "importe",
    "fechaDeposito",
    "tipoDocumento",
    "serieDocumento",
    "numeroDocumento",
    "utilizado",
  ];

  dataSource = new MatTableDataSource(this.depositoDetalle);
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditDepositoBancoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DepositoBanco,
    public fb: FormBuilder,
    public depositoBancoService: DepositoBancoService,
    public cuentaCorrienteService: CuentaCorrienteService
  ) {
    this.form = this.fb.group({
      cuentaCorriente: [data.cuentaCorriente?.numero + ' - ' + data.cuentaCorriente?.denominacion],
      fechaDeposito:[data?.fechaDeposito],
      nombreArchivo: [data?.nombreArchivo]
    });
  }

  ngOnInit() {
    this.onLoad();
  }
  onLoad() {
    this.depositoBancoService.getDepositoBancoById(this.data.depositoBancoId)
      .subscribe((response) => {
        if (response.success) {
          this.dataSource.data = response.data.depositoBancoDetalle;
          this.dataSource._updateChangeSubscription();
        }
      });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
