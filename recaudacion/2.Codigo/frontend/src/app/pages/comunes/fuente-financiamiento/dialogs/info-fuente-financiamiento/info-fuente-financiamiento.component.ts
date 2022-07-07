import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FuenteFinanciamiento } from 'src/app/core/models/fuentefinanciamiento';


@Component({
  selector: 'app-info-fuente-financiamiento',
  templateUrl: './info-fuente-financiamiento.component.html',
  styleUrls: ['./info-fuente-financiamiento.component.scss']
})
export class InfoFuenteFinanciamientoComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoFuenteFinanciamientoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FuenteFinanciamiento,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      codigo: [data?.codigo],
      descripcion: [data?.descripcion],
      rubroCodigo: [data?.rubroCodigo],
      rubroDescripcion: [data?.rubroDescripcion],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {
    
  }

}
