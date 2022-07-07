import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { TipoCaptacion } from 'src/app/core/models/tipocaptacion';

@Component({
  selector: 'app-info-tipo-captacion',
  templateUrl: './info-tipo-captacion.component.html',
  styleUrls: ['./info-tipo-captacion.component.scss']
})
export class InfoTipoCaptacionComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoTipoCaptacionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoCaptacion,
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
