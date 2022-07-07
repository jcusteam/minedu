import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UnidadEjecutora } from 'src/app/core/models/unidadejecutora';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { PideService } from 'src/app/core/services/pide.service';
import { UnidadEjecutoraService } from 'src/app/core/services/unidad-ejecutora.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-edit-unidad-ejecutora',
  templateUrl: './edit-unidad-ejecutora.component.html',
  styleUrls: ['./edit-unidad-ejecutora.component.scss']
})
export class EditUnidadEjecutoraComponent implements OnInit {

  form: FormGroup;
  unidadEjecutora = new UnidadEjecutora();

  constructor(
    public dialogRef: MatDialogRef<EditUnidadEjecutoraComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UnidadEjecutora,
    public fb: FormBuilder,
    private UnidadEjecutoraService: UnidadEjecutoraService,
    private messageService: MessageService,
    private authService: AuthService,
    private pideService: PideService,
  ) {
    this.form = this.fb.group({
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      secuencia: [
        data?.secuencia,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10)
        ]),
      ],
      codigo: [
        data?.codigo,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10)
        ]),
      ],
      nombre: [
        data?.nombre,
        Validators.compose([
          Validators.required,
          Validators.maxLength(200),
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
        ]),
      ],
      numeroRuc: [
        data?.numeroRuc,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]{11,11}$"),
          Validators.maxLength(11)
        ]),
      ],
      direccion: [
        data?.direccion,
        Validators.compose([
          Validators.required,
          Validators.maxLength(300)
        ])
      ],
      correo: [
        data?.correo,
        Validators.compose([
          Validators.required,
          Validators.email,
          Validators.maxLength(100)
        ]
        ),
      ],
      telefono: [data?.telefono],
      celular: [data?.celular],
      estado: [data.estado],
      usuarioModificador: [data.usuarioModificador]
    });
  }
  ngOnInit() {

  }

  // Limpiar formulario
  clearForm() {
    this.form.patchValue({
      nombre: null,
      codigo: null,
      secuencia: null,
      direccion: null,
      correo: null
    });
  }

  // Key Up Ruc
  onKeyUpRuc(nro) {
    this.clearForm();
  }

  // Buscar RUC
  searchRuc() {
    this.clearForm();

    if (!this.form.get("numeroRuc").valid) {
      return;
    }

    let numeroRuc = this.form.get("numeroRuc").value;
    this.getSunatData(numeroRuc);
  }

  // Consulta SUNAT
  getSunatData(numeroRuc) {
    this.messageService.msgLoad("Consultado SUNAT...");
    this.pideService.getSunatByRuc(numeroRuc).subscribe(
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

  onSubmit(form: UnidadEjecutora) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let unidadEjecutora = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.UnidadEjecutoraService.updateUnidadEjecutora(unidadEjecutora).subscribe(
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
