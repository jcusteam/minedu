import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoDocumentoIdentidad } from 'src/app/core/models/tipodocumentoidentidad';


@Component({
  selector: 'app-info-tipo-documento-identidad',
  templateUrl: './info-tipo-documento-identidad.component.html',
  styleUrls: ['./info-tipo-documento-identidad.component.scss']
})
export class InfoTipoDocumentoIdentidadComponent implements OnInit {

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoTipoDocumentoIdentidadComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoDocumentoIdentidad,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      codigo: [data?.codigo],
      nombre: [data?.nombre],
      descripcion: [data?.descripcion],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {

  }

}
