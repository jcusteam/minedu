import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoComprobantePago } from 'src/app/core/models/tipocomprobantepago';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TipoComprobantePagoService } from 'src/app/core/services/tipo-comprobante-pago.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';


@Component({
  selector: 'app-new-tipo-comprobante-pago',
  templateUrl: './new-tipo-comprobante-pago.component.html',
  styleUrls: ['./new-tipo-comprobante-pago.component.scss']
})
export class NewTipoComprobantePagoComponent implements OnInit {

  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewTipoComprobantePagoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoComprobantePago,
    public fb: FormBuilder,
    private tipoComprobantePagoService: TipoComprobantePagoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      codigo: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.maxLength(2)
        ])
      ],
      nombre: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(100)
        ])
      ],
      estado: [true],
      usuarioCreador:[data?.usuarioCreador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: TipoComprobantePago) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let tipoComprobantePago = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.tipoComprobantePagoService.createTipoComprobantePago(tipoComprobantePago).subscribe(
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
