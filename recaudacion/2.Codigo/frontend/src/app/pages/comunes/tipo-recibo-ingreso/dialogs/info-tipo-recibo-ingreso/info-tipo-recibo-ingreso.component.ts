import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoReciboIngreso } from 'src/app/core/models/tiporeciboingreso';

@Component({
  selector: 'app-info-tipo-recibo-ingreso',
  templateUrl: './info-tipo-recibo-ingreso.component.html',
  styleUrls: ['./info-tipo-recibo-ingreso.component.scss']
})
export class InfoTipoReciboIngresoComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoTipoReciboIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoReciboIngreso,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      nombre: [data?.nombre],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {

  }

}
