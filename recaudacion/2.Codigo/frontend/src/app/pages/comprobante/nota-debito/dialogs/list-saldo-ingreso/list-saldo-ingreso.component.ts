import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { IngresoPecosaService } from 'src/app/core/services/ingreso-pecosa.service';
import { IngresoPecosaDetalle } from 'src/app/core/models/ingresopecosa';
import { CatalogoBien } from 'src/app/core/models/catalogobien';

@Component({
  selector: 'app-list-saldo-ingreso',
  templateUrl: './list-saldo-ingreso.component.html',
  styleUrls: ['./list-saldo-ingreso.component.scss']
})
export class ListSaldoIngresoNotaDebitoComponent implements OnInit {

  ingresoPecosaDetalles: IngresoPecosaDetalle[] = [];
  dataSource = new MatTableDataSource(this.ingresoPecosaDetalles);

  displayedColumns: string[] = [
    'nro', 'anioPecosa', 'numeroPecosa', 'serieFormato', 'serieDel', 'serieAl', 'saldo', 'cantidadSalida', 'serieDelSalida', 'serieAlSalida'
  ];

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<ListSaldoIngresoNotaDebitoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CatalogoBien,
    private fb: FormBuilder,
    private ingresoPecosaService: IngresoPecosaService) {
    this.form = this.fb.group({
      catalogoBien: [data?.codigo + " - " + data?.descripcion],
      detalles: this.fb.array([])
    });

  }

  ngOnInit() {
    this.setDetallesForm();
    this.onLoadData();

  }

  onLoadData() {
    this.ingresoPecosaService.getIngresoPecosaDetalleSaldos(this.data.catalogoBienId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach(row => {
            //row.saldo = row.cantidad - row.cantidadSalida;
            row.cantidadSalida = null;
            row.serieDelSalida = null;
            row.serieAlSalida = null;
          });
          this.dataSource.data = response.data;
          this.dataSource._updateChangeSubscription();
          this.setDetallesForm();
        }
      });
  }

  private setDetallesForm() {
    const detalleCtrl = this.form.get('detalles') as FormArray;
    this.dataSource.data.forEach((detalle) => {
      detalleCtrl.push(this.setDetalleFormArray(detalle))
    })
  };

  private setDetalleFormArray(detalle: IngresoPecosaDetalle) {

    return this.fb.group({
      codigoItem: [detalle?.codigoItem],
      nombreItem: [detalle?.nombreItem],
      nombreMarca: [detalle?.nombreMarca],
      cantidad: [detalle?.cantidad],
      precioUnitario: [detalle?.precioUnitario],
      valorTotal: [detalle?.valorTotal],
      serieFormato: [detalle?.serieFormato],
      cantidadSalida: [
        detalle?.cantidadSalida,
        Validators.compose([
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10),
          Validators.min(1),
          Validators.max(detalle?.saldo),
        ])
      ],
      serieDelSalida: [
        detalle?.serieDelSalida,
        Validators.compose([
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10),
          Validators.min(1)
        ])
      ],
      serieAlSalida: [
        detalle?.serieAlSalida,
        Validators.compose([
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10),
          Validators.min(1)
        ])]
    });
  }

  applyCantidadSalida(event: Event, item) {
    var cantidadSalida = (event.target as HTMLInputElement).value;
    var letters = /^[0-9]+$/;

    if (!cantidadSalida.match(letters)) {
      (event.target as HTMLInputElement).value = '';
      cantidadSalida = '0';
    }

    this.dataSource.data.forEach(row => {
      if (row.ingresoPecosaId == item.ingresoPecosaId) {
        row.cantidadSalida = +cantidadSalida;
      }
    });
    this.dataSource._updateChangeSubscription();
  }

  applySerieDel(event: Event, item) {
    var serieDel = (event.target as HTMLInputElement).value;
    var letters = /^[0-9]+$/;
    if (!serieDel.match(letters)) {
      (event.target as HTMLInputElement).value = '';
      serieDel = '0';
    }

    this.dataSource.data.forEach(row => {
      if (row.ingresoPecosaId == item.ingresoPecosaId) {
        row.serieDelSalida = +serieDel;
      }
    });
    this.dataSource._updateChangeSubscription();
  }


  applySerieAl(event: Event, item) {
    var serieAl = (event.target as HTMLInputElement).value;

    var letters = /^[0-9]+$/;
    if (!serieAl.match(letters)) {
      (event.target as HTMLInputElement).value = '';
      serieAl = '0';
    }

    this.dataSource.data.forEach(row => {
      if (row.ingresoPecosaId == item.ingresoPecosaId) {
        row.serieAlSalida = +serieAl;
      }
    });

    this.dataSource._updateChangeSubscription();
  }

  onSubmit(form) {
    if (!this.form.valid) {
      return;
    }

    const total = this.dataSource.data.filter(x => x.cantidadSalida > 0).length;
    if (total == 0)
      return;

    this.closeDialog();
  }

  closeDialog(): void {
    this.dialogRef.close(this.dataSource.data);
  }
}
