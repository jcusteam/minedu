import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UnidadMedida } from 'src/app/core/models/unidadmedida';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { UnidadMedidaService } from 'src/app/core/services/unidad-medida.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-new-unidad-medida',
  templateUrl: './new-unidad-medida.component.html',
  styleUrls: ['./new-unidad-medida.component.scss']
})
export class NewUnidadMedidaComponent implements OnInit {

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewUnidadMedidaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UnidadMedida,
    public fb: FormBuilder,
    private unidadMedidaService: UnidadMedidaService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
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
          Validators.maxLength(6)
        ])
      ],
      estado: [true],
      usuarioCreador:[data?.usuarioCreador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: UnidadMedida) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let unidadMedida = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.unidadMedidaService.createUnidadMedida(unidadMedida).subscribe(
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
