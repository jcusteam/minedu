import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { ClienteService } from "src/app/core/services/cliente.service";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Cliente } from "src/app/core/models/cliente";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { PideService } from "src/app/core/services/pide.service";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";

@Component({
  selector: "app-new-cliente",
  templateUrl: "./new-cliente.component.html",
  styleUrls: ["./new-cliente.component.scss"],
})
export class NewClienteComponent implements OnInit {
  form: FormGroup;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoDocumentos: TipoDocumentoIdentidad[] = [];

  constructor(
    public dialogRef: MatDialogRef<NewClienteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Cliente,
    private clienteService: ClienteService,
    private tipoDocumentoIdentidadSercice: TipoDocumentoIdentidadService,
    private pideService: PideService,
    private fb: FormBuilder,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      tipoDocumentoIdentidadId: [
        this.tipoDocIdentidadEnum.DNI,
        Validators.compose([Validators.required]),
      ],
      numeroDocumento: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]{8,8}$")
        ]),
      ],
      nombre: [
        null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(200)
        ]),
      ],
      direccion: [null,
        Validators.compose([
          Validators.required,
          Validators.maxLength(300)
        ])
      ],
      correo: [null, 
        Validators.compose([
          Validators.email,
          Validators.maxLength(100)
        ])
      ],
      estado: [true],
      usuarioCreador: [data.usuarioCreador],
    });
  }

  ngOnInit() {
    this.tipoDocumentoIdentidadSercice.getTipoDocumentoIdentidades().subscribe(
      (response) => {
        this.tipoDocumentos = response.data;
      });
  }

  onSelectionTipoDocIdentidad(id: number) {
    this.form.patchValue({
      numeroDocumento: null,
      nombre: null,
      correo: null,
      direccion: null
    });
    this.valiationTipoDocIdentidad(id);
  }

  valiationTipoDocIdentidad(id: number) {
    if (id == this.tipoDocIdentidadEnum.DNI) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{8,8}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.CE) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.RUC) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{11,11}$"),
        ]);
      this.form.updateValueAndValidity();
    } else {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    }
  }

  // Limpiar formulario
  clearForm() {
    this.form.patchValue({
      nombre: null,
      correo: null,
      direccion: null
    });
  }

  // Key Up Nro Doc
  onKeyUpNroDoc(nro) {
    this.clearForm();
  }

  searchNroDoc() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.clearForm();

    if (!this.form.get("tipoDocumentoIdentidadId").valid) {
      return;
    }

    if (!this.form.get("numeroDocumento").valid) {
      return;
    }

    const numeroDocumento = this.form.get("numeroDocumento").value;
    const tipoDocumentoIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;

    if (tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI) {
      this.getReniecData(numeroDocumento);

    } else if (tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE) {
      this.getMigracionData(numeroDocumento);

    } else if (tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.RUC) {
      this.getSunatData(numeroDocumento);
    }
  }

  // Consulta SUNAT
  getReniecData(numeroDoc) {
    this.messageService.msgLoad("Consultado RENIEC...");
    this.pideService.getReniecByDni(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          var nombre = response.data.nombreCompleto;
          var direccion = response.data.domicilioApp;
          this.form.patchValue({
            nombre: nombre,
            direccion: direccion
          });
        }
        else {
          var message = response.messages.join(",");
          let type = response.messageType;
          if (type == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          }
          else if (type == TYPE_MESSAGE.INFO) {
            this.messageService.msgWarning(message, () => { });
          }
          else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => {
        this.messageService.msgError(MESSAGES.API.ERR_SERVICE, () => { });
      }
    );
  }

  // Consulta MIGRACION
  getMigracionData(numeroDoc) {
    this.messageService.msgLoad("Consultado Migraciones...");
    this.pideService.getMigracionByNro(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          var nombre = response.data.strNombreCompleto;
          var direccion = "";
          this.form.patchValue({
            nombre: nombre,
            direccion: direccion
          });
        }
        else {
          var message = response.messages.join(",");
          let type = response.messageType;
          if (type == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          }
          else if (type == TYPE_MESSAGE.INFO) {
            this.messageService.msgWarning(message, () => { });
          }
          else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => {
        this.messageService.msgError(MESSAGES.API.ERR_SERVICE, () => { });
      }
    );
  }

  // Consulta SUNAT
  getSunatData(numeroDoc) {
    this.messageService.msgLoad("Consultado SUNAT...");
    this.pideService.getSunatByRuc(numeroDoc).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response.success) {
          var nombre = response.data.ddp_nombre;
          var direccion = response.data.desc_domi_fiscal;
          this.form.patchValue({
            nombre: nombre,
            direccion: direccion
          });
        }
        else {
          var message = response.messages.join(",");
          let type = response.messageType;
          if (type == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          }
          else if (type == TYPE_MESSAGE.INFO) {
            this.messageService.msgWarning(message, () => { });
          }
          else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => {
        this.messageService.msgError(MESSAGES.API.ERR_SERVICE, () => { });
      }
    );
  }

  onSubmit(form: Cliente) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let cliente = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.clienteService.createCliente(cliente).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {

      });

  }

  handleResponse(type, message, success) {
    if (success) {
      this.messageService.msgSuccess(message, () => {
        this.onNoClick();
      });
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { });
      }
      else {
        this.messageService.msgError(message, () => { });
      }
    }
  }
  
  handleError(error) {
    throw error; 
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onCloseDialog() {
    this.messageService.msgClose(MESSAGES.FORM.CLOSE_FORM, () => {
      this.onNoClick();
    });
  }

  onNoClick(): void {
    this.dialogRef.close(0);
  }

}
