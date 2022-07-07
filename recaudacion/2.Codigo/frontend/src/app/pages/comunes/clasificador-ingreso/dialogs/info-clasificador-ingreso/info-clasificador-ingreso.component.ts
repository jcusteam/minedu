import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ClasificadorIngreso } from 'src/app/core/models/clasificadoringreso';
import { CuentaContableService } from 'src/app/core/services/cuenta-contable.service';


@Component({
  selector: 'app-info-clasificador-ingreso',
  templateUrl: './info-clasificador-ingreso.component.html',
  styleUrls: ['./info-clasificador-ingreso.component.scss']
})
export class InfoClasificadorIngresoComponent implements OnInit {

  form: FormGroup;
  cuentasContables = [];

  constructor(
    public dialogRef: MatDialogRef<InfoClasificadorIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ClasificadorIngreso,
    public fb: FormBuilder,
    private cuentaContableService: CuentaContableService
  ) {
    this.form = this.fb.group({
      cuentaContableIdDebe: [data.cuentaContableIdDebe,],
      cuentaContableIdHaber: [data.cuentaContableIdHaber,],
      codigo: [data.codigo],
      descripcion: [data.descripcion],
      tipoTransaccion: [data?.tipoTransaccion],
      generica: [data?.generica],
      subGenerica: [data?.subGenerica],
      subGenericaDetalle: [data?.subGenericaDetalle],
      especifica: [data?.especifica],
      especificaDetalle: [data?.especificaDetalle],
      estado: [data.estado],
    });
  }
  ngOnInit() {
    
    this.cuentaContableService.getCuentaContables().subscribe(
      (response) => {
        if (response.success) {
          this.cuentasContables = response.data;
        }
      }
    );
  }

  onSubmit() {
   
  }
}
