import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Cliente } from "src/app/core/models/cliente";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";

@Component({
  selector: 'app-info-cliente',
  templateUrl: './info-cliente.component.html',
  styleUrls: ['./info-cliente.component.scss']
})
export class InfoClienteComponent implements OnInit {
  form: FormGroup;
  tipoDocumentos: TipoDocumentoIdentidad[] = [];

  constructor(
    public dialogRef: MatDialogRef<InfoClienteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Cliente,
    private fb: FormBuilder,
    private tipoDocumentoIdentidadSercice: TipoDocumentoIdentidadService
  ) {
    this.form = this.fb.group({
      tipoDocumentoIdentidadId: [data?.tipoDocumentoIdentidadId],
      numeroDocumento: [data?.numeroDocumento],
      nombre: [data?.nombre],
      direccion: [data?.direccion],
      correo: [data?.correo],
    });
  }

  ngOnInit() {

    this.tipoDocumentoIdentidadSercice.getTipoDocumentoIdentidades().subscribe(
      (response) => {
        this.tipoDocumentos = response.data;
      }
    );
  }

  onSubmit() {

  }

}
