import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoDocumento } from 'src/app/core/models/tipodocumento';

@Component({
  selector: 'app-info-tipo-documento',
  templateUrl: './info-tipo-documento.component.html',
  styleUrls: ['./info-tipo-documento.component.scss']
})
export class InfoTipoDocumentoComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoTipoDocumentoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoDocumento,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      nombre: [data?.nombre],
      abreviatura: [data?.abreviatura],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit(form: TipoDocumento) {
    
  }


}
