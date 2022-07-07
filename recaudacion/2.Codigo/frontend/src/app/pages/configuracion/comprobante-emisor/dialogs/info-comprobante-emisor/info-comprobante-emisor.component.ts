import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ComprobanteEmisor } from 'src/app/core/models/comprobante-emisor';
import { ComprobanteEmisorService } from 'src/app/core/services/comprobante-emisor.service';

@Component({
  selector: 'app-info-comprobante-emisor',
  templateUrl: './info-comprobante-emisor.component.html',
  styleUrls: ['./info-comprobante-emisor.component.scss']
})
export class InfoComprobanteEmisorComponent implements OnInit {

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<InfoComprobanteEmisorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobanteEmisor,
    public fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      firmante: [data?.firmante],
      numeroRuc: [data?.numeroRuc],
      tipoDocumento: [data?.tipoDocumento],
      nombreComercial: [data?.nombreComercial],
      razonSocial: [data?.razonSocial],
      ubigeo: [data?.ubigeo],
      direccion: [data?.direccion],
      urbanizacion: [data?.urbanizacion],
      departamento: [data?.departamento],
      provincia: [data?.provincia],
      distrito: [data?.distrito],
      codigoPais: [data?.codigoPais],
      telefono: [data?.telefono],
      direccionAlternativa: [data?.direccionAlternativa],
      numeroResolucion: [data?.numeroResolucion],
      usuarioOSE: [data?.usuarioOSE],
      claveOSE: [data?.claveOSE],
      correoEnvio: [data?.correoEnvio],
      correoClave: [data?.correoClave],
      serverMail: [data?.serverMail],
      serverPort: [data?.serverPort],
      nombreArchivoCer: [data?.nombreArchivoCer],
      nombreArchivoKey: [data?.nombreArchivoKey],
      estado: [data?.estado],
    });
  }
  ngOnInit() {

  }

  onSubmit() {

  }


}
