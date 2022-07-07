import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CuentaContable } from 'src/app/core/models/cuentacontable';

@Component({
  selector: 'app-info-cuenta-contable',
  templateUrl: './info-cuenta-contable.component.html',
  styleUrls: ['./info-cuenta-contable.component.scss']
})
export class InfoCuentaContableComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoCuentaContableComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CuentaContable,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      codigo: [data?.codigo.trim()],
      descripcion: [data?.descripcion],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {
    
  }


}
