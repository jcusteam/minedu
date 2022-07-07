import { Component, OnInit, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { RegistroLinea, RegistroLineaEstado } from "src/app/core/models/registrolinea";
import { RegistroLineaService } from "src/app/core/services/registro-linea.service";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-obs-registro-linea",
  templateUrl: "./obs-registro-linea.component.html",
  styleUrls: ["./obs-registro-linea.component.scss"],
})
export class ObsRegistroLineaComponent implements OnInit {
  form: FormGroup;
  constructor(
    public dialogRef: MatDialogRef<ObsRegistroLineaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RegistroLineaEstado,
    private fb: FormBuilder,
    private registroLineaService: RegistroLineaService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      observacion: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
          Validators.maxLength(300)
        ])
      ],
    });
  }

  ngOnInit() { }

  onSubmit(form: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if(!this.form.valid){
      return;
    }
    
    this.data.observacion = form.observacion;
    this.messageService.msgConfirm(MESSAGES.FORM.REGISTRO_LINEA.CONFIRM_OBSERVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");

        this.registroLineaService.updateEstadoRegistroLinea(this.data).subscribe(
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
