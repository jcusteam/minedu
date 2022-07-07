import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Estado } from 'src/app/core/models/estado';
import { AuthService } from 'src/app/core/services/auth.service';
import { EstadoService } from 'src/app/core/services/estado.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';


@Component({
  selector: 'app-edit-tipo-documento-estado',
  templateUrl: './edit-tipo-documento-estado.component.html',
  styleUrls: ['./edit-tipo-documento-estado.component.scss']
})
export class EditTipoDocumentoEstadoComponent implements OnInit {

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditTipoDocumentoEstadoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Estado,
    public fb: FormBuilder,
    private estadoService: EstadoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {

    this.form = this.fb.group({
      estadoId: [data?.estadoId],
      numero: [data?.numero,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.min(1),
          Validators.max(10),
        ])
      ],
      orden: [data?.orden,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.min(1)
        ])
      ],
      nombre: [data?.nombre,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      tipoDocumentoId:[data?.tipoDocumentoId],
      usuarioModificador: [data?.usuarioModificador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: Estado) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let estado = form;
    estado.numero = +form.numero;
    estado.orden = +form.orden;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");

        this.estadoService.updateEstado(estado).subscribe(
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
