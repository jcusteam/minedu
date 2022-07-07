import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Banco } from 'src/app/core/models/banco';

@Component({
  selector: 'app-info-banco',
  templateUrl: './info-banco.component.html',
  styleUrls: ['./info-banco.component.scss']
})
export class InfoBancoComponent implements OnInit {
  form: FormGroup;
  constructor(
    public dialogRef: MatDialogRef<InfoBancoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Banco,
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
