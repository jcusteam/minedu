import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoComprobantePago } from 'src/app/core/models/tipocomprobantepago';

@Component({
  selector: 'app-info-tipo-comprobante-pago',
  templateUrl: './info-tipo-comprobante-pago.component.html',
  styleUrls: ['./info-tipo-comprobante-pago.component.scss']
})
export class InfoTipoComprobantePagoComponent implements OnInit {

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoTipoComprobantePagoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoComprobantePago,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      codigo: [data?.codigo],
      nombre: [data?.nombre],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {
    
  }

}
