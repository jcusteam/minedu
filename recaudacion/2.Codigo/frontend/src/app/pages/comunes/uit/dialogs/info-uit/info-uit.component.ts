
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Uit } from 'src/app/core/models/uit';


@Component({
  selector: 'app-info-uit',
  templateUrl: './info-uit.component.html',
  styleUrls: ['./info-uit.component.scss']
})
export class InfoUitComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoUitComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Uit,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      periodo: [data?.periodo],
      unidadMonetaria: [data?.unidadMonetaria],
      valor: [data?.valor],
      porcentaje: [data?.porcentaje],
      baseLegal: [data?.baseLegal],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {

  }


}

