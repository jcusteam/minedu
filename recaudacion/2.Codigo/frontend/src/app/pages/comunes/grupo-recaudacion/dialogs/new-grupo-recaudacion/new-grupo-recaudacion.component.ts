import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GrupoRecaudacion } from 'src/app/core/models/gruporecaudacion';
import { AuthService } from 'src/app/core/services/auth.service';
import { GrupoRecaudacionService } from 'src/app/core/services/grupo-recaudacion.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-new-grupo-recaudacion',
  templateUrl: './new-grupo-recaudacion.component.html',
  styleUrls: ['./new-grupo-recaudacion.component.scss']
})
export class NewGrupoRecaudacionComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<NewGrupoRecaudacionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: GrupoRecaudacion,
    public fb: FormBuilder,
    private grupoRecaudacionService: GrupoRecaudacionService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      nombre: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(100)
        ])
      ],
      estado: [true],
      usuarioCreador: [data.usuarioCreador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: GrupoRecaudacion) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let grupoRecaudacion = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.grupoRecaudacionService.createGrupoRecaudacion(grupoRecaudacion).subscribe(
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
