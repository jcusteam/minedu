import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GrupoRecaudacion } from 'src/app/core/models/gruporecaudacion';


@Component({
  selector: 'app-info-grupo-recaudacion',
  templateUrl: './info-grupo-recaudacion.component.html',
  styleUrls: ['./info-grupo-recaudacion.component.scss']
})
export class InfoGrupoRecaudacionComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoGrupoRecaudacionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: GrupoRecaudacion,
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
