import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { CuentaContable } from "src/app/core/models/cuentacontable";
import { AuthService } from "src/app/core/services/auth.service";
import { CuentaContableService } from "src/app/core/services/cuenta-contable.service";
import { MessageService } from "src/app/core/services/message.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-edit-cuenta-contable",
  templateUrl: "./edit-cuenta-contable.component.html",
  styleUrls: ["./edit-cuenta-contable.component.scss"],
})
export class EditCuentaContableComponent implements OnInit {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<EditCuentaContableComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CuentaContable,
    public fb: FormBuilder,
    private cuentaContableService: CuentaContableService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      cuentaContableId:[data.cuentaContableId],
      codigo: [
        data?.codigo.trim(),
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(15)
        ]),
      ],
      descripcion: [
        data?.descripcion,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ]),
      ],
      estado: [data.estado],
      usuarioModificador:[data.usuarioModificador]
    });
  }
  ngOnInit() {}

  onSubmit(form: CuentaContable) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let cuentaContable = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.cuentaContableService.updateCuentaContable(cuentaContable).subscribe(
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
