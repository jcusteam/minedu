import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoReciboIngreso } from 'src/app/core/models/tiporeciboingreso';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TipoReciboIngresoService } from 'src/app/core/services/tipo-recibo-ingreso.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-new-tipo-recibo-ingreso',
  templateUrl: './new-tipo-recibo-ingreso.component.html',
  styleUrls: ['./new-tipo-recibo-ingreso.component.scss']
})
export class NewTipoReciboIngresoComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewTipoReciboIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoReciboIngreso,
    public fb: FormBuilder,
    private tipoReciboIngresoService: TipoReciboIngresoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      nombre: [null, Validators.compose([
        Validators.required,
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(50)
      ])],
      estado: [true],
      usuarioCreador:[data?.usuarioCreador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: TipoReciboIngreso) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let tipoReciboIngreso = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.tipoReciboIngresoService.createTipoReciboIngreso(tipoReciboIngreso).subscribe(
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
