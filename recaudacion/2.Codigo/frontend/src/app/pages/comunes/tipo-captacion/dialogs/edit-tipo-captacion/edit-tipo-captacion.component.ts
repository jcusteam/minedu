import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoCaptacion } from 'src/app/core/models/tipocaptacion';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TipoCaptacionService } from 'src/app/core/services/tipo-captacion.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-edit-tipo-captacion',
  templateUrl: './edit-tipo-captacion.component.html',
  styleUrls: ['./edit-tipo-captacion.component.scss']
})
export class EditTipoCaptacionComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditTipoCaptacionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoCaptacion,
    public fb: FormBuilder,
    private tipoCaptacionService: TipoCaptacionService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      tipoCaptacionId:[data?.tipoCaptacionId],
      nombre: [data?.nombre,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
      estado: [data?.estado],
      usuarioModificador:[data?.usuarioModificador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: TipoCaptacion) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let tipoCaptacion = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.tipoCaptacionService.updateTipoCaptacion(tipoCaptacion).subscribe(
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
