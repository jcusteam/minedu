import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoDocumento } from 'src/app/core/models/tipodocumento';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TipoDocumentoService } from 'src/app/core/services/tipo-documento.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-new-tipo-documento',
  templateUrl: './new-tipo-documento.component.html',
  styleUrls: ['./new-tipo-documento.component.scss']
})
export class NewTipoDocumentoComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewTipoDocumentoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoDocumento,
    public fb: FormBuilder,
    private tipoDocumentoService: TipoDocumentoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      tipoDocumentoId:[0],
      nombre: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      abreviatura: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(10)
        ])
      ],
      estado: [data?.estado],
      usuarioCreador: [data?.usuarioCreador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: TipoDocumento) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let tipoDocumento = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.tipoDocumentoService.createTipoDocumento(tipoDocumento).subscribe(
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
