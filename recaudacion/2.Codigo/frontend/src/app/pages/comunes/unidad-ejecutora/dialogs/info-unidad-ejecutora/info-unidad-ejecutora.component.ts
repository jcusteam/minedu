import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UnidadEjecutora } from 'src/app/core/models/unidadejecutora';

@Component({
  selector: 'app-info-unidad-ejecutora',
  templateUrl: './info-unidad-ejecutora.component.html',
  styleUrls: ['./info-unidad-ejecutora.component.scss']
})
export class InfoUnidadEjecutoraComponent implements OnInit {

  unidadEjecutoraForm: FormGroup;
  unidadEjecutora = new UnidadEjecutora();

  constructor(
    public dialogRef: MatDialogRef<InfoUnidadEjecutoraComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UnidadEjecutora,
    public fb: FormBuilder,
  ) {
    this.unidadEjecutoraForm = this.fb.group({
      secuencia: [data?.secuencia],
      codigo: [data.codigo],
      nombre: [data.nombre],
      numeroRuc: [data.numeroRuc],
      direccion: [data.direccion],
      correo: [data.correo],
      estado: [data.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {

  }


}
