import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FuenteFinanciamiento } from 'src/app/core/models/fuentefinanciamiento';
import { AuthService } from 'src/app/core/services/auth.service';
import { FuenteFinanciamientoService } from 'src/app/core/services/fuente-financiamiento.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-edit-fuente-financiamiento',
  templateUrl: './edit-fuente-financiamiento.component.html',
  styleUrls: ['./edit-fuente-financiamiento.component.scss']
})
export class EditFuenteFinanciamientoComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditFuenteFinanciamientoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FuenteFinanciamiento,
    public fb: FormBuilder,
    private fuenteFinanciamientoService: FuenteFinanciamientoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      fuenteFinanciamientoId: [data?.fuenteFinanciamientoId],
      codigo: [data?.codigo,
      Validators.compose([
        Validators.required,
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(2)
      ])
      ],
      descripcion: [data?.descripcion,
      Validators.compose([
        Validators.required,
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(100)
      ])
      ],
      rubroCodigo: [data?.rubroCodigo,
      Validators.compose([
        Validators.required,
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(10)
      ])
      ],
      rubroDescripcion: [data?.rubroDescripcion,
      Validators.compose([
        Validators.required,
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(100)
      ])
      ],
      estado: [data.estado],
      usuarioModificador: [data.usuarioModificador]
    });
  }
  ngOnInit() {

  }

  onSubmit(form: FuenteFinanciamiento) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let fuenteFinanciamiento = form;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.fuenteFinanciamientoService.updateFuenteFinanciamiento(fuenteFinanciamiento).subscribe(
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
