import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Parametro } from 'src/app/core/models/parametro';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { ParametroService } from 'src/app/core/services/parametro.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-edit-tipo-documento-parametro',
  templateUrl: './edit-tipo-documento-parametro.component.html',
  styleUrls: ['./edit-tipo-documento-parametro.component.scss']
})
export class EditTipoDocumentoParametroComponent implements OnInit {
  created = false;
  form: FormGroup;
  constructor(
    public dialogRef: MatDialogRef<EditTipoDocumentoParametroComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Parametro,
    public fb: FormBuilder,
    private parametroService: ParametroService,
    private messageService: MessageService,
    private authService: AuthService
  ) {

    this.form = this.fb.group({
      parametroId: [0],
      unidadEjecutoraId: [data.unidadEjecutoraId],
      tipoDocumentoId: [data.tipoDocumentoId],
      serie: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-z0-9]+$'),
          Validators.maxLength(4)
        ])
      ],
      correlativo: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.maxLength(12)
        ])
      ],
      estado: [data?.estado],
      usuarioCreador: [data?.usuarioCreador],
      usuarioModificador: [data?.usuarioModificador]
    });
  }
  ngOnInit() {

    this.parametroService.getParametroByUnidadTipo(this.data.unidadEjecutoraId, this.data.tipoDocumentoId)
      .subscribe((response) => {
        if (response.success) {
          this.created = false;
          let parametro = response.data;
          this.form.patchValue({
            parametroId: parametro?.parametroId,
            serie: parametro?.serie.trim(),
            correlativo: parametro?.correlativo.trim(),
            estado: parametro?.estado
          });
        }
        else {
          this.created = true;
        }
      }
      );
  }

  onSubmit(form: Parametro) {


    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      return;
    }

    let parametro = form;

    if (this.created) {
      this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
        () => {
          this.messageService.msgLoad("Guardando...");
          this.parametroService.createParametro(parametro).subscribe(
            (response) => {
              var message = response.messages.join(",");
              this.handleResponse(response.messageType, message, response.success);
            },
            (error) => this.handleError(error)
          );
        }, () => {

        });
    }
    else {
      this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
        () => {
          this.messageService.msgLoad("Actualizando...");
          this.parametroService.updateParametro(parametro).subscribe(
            (response) => {
              var message = response.messages.join(",");
              this.handleResponse(response.messageType, message, response.success);
            },
            (error) => this.handleError(error)
          );
        }, () => {

        });
    }


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
