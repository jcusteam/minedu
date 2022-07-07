import { Component, OnInit, Inject } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { DatePipe } from '@angular/common';
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IngresoPecosa, IngresoPecosaDetalle } from 'src/app/core/models/ingresopecosa';
import { IngresoPecosaService } from 'src/app/core/services/ingreso-pecosa.service';

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
  selector: 'app-info-ingreso-pecosa',
  templateUrl: './info-ingreso-pecosa.component.html',
  styleUrls: ['./info-ingreso-pecosa.component.scss'],
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
export class InfoIngresoPecosaComponent implements OnInit {

  displayedColumns: string[] =
    [
      'nro', 'codigoItem', 'nombreItem', 'nombreMarca', 'cantidad',
      'precioUnitario', 'valorTotal', 'serieFormato', 'serieDel', 'serieAl'
    ];
  form: FormGroup;
  ingresoPecosaDetalles: IngresoPecosaDetalle[] = [];
  dataSource = new MatTableDataSource(this.ingresoPecosaDetalles);

  constructor(public dialogRef: MatDialogRef<InfoIngresoPecosaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IngresoPecosa,
    private fb: FormBuilder,
    private ingresoPecosaService: IngresoPecosaService,) {
    this.form = this.fb.group({
      numeroPecosa: data?.numeroPecosa,
      anioPecosa: data?.anioPecosa,
      fechaPecosa: data?.fechaPecosa,
      tipoBien: data?.tipoBien,
      nombreAlmacen: data?.nombreAlmacen,
      motivoPedido: data?.motivoPedido,
      fechaRegistro: data?.fechaRegistro
    });
  }

  ngOnInit() {
    this.ingresoPecosaService.getIngresoPecosaById(this.data.ingresoPecosaId).subscribe(
      (response) => {
        if (response.success) {
          this.dataSource.data = response.data.ingresoPecosaDetalle;
        }
      }
    );
  }

  onSubmit() {

  }
}
