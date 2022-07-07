import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UnidadMedida } from 'src/app/core/models/unidadmedida';


@Component({
  selector: 'app-info-unidad-medida',
  templateUrl: './info-unidad-medida.component.html',
  styleUrls: ['./info-unidad-medida.component.scss']
})
export class InfoUnidadMedidaComponent implements OnInit {

  unidadMedidaForm: FormGroup;
  unidadMedida = new UnidadMedida();

  constructor(
    public dialogRef: MatDialogRef<InfoUnidadMedidaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UnidadMedida,
    public fb: FormBuilder  ) {
    this.unidadMedidaForm = this.fb.group({
      nombre: [data.nombre],
      abreviatura: [data.abreviatura],
      estado: [data.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {
   
  }

}
