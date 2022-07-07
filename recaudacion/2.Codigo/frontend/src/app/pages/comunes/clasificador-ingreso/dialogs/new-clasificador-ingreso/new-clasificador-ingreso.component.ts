import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import { CuentaContable } from "src/app/core/models/cuentacontable";
import { AuthService } from "src/app/core/services/auth.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { CuentaContableService } from "src/app/core/services/cuenta-contable.service";
import { MessageService } from "src/app/core/services/message.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-new-clasificador-ingreso",
  templateUrl: "./new-clasificador-ingreso.component.html",
  styleUrls: ["./new-clasificador-ingreso.component.scss"],
})
export class NewClasificadorIngresoComponent implements OnInit {
  form: FormGroup;
  cuentasContables: CuentaContable[] = [];

  constructor(
    public dialogRef: MatDialogRef<NewClasificadorIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ClasificadorIngreso,
    public fb: FormBuilder,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private cuentaContableService: CuentaContableService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      cuentaContableIdDebe: [null, Validators.compose([Validators.required])],
      cuentaContableIdHaber: [null, Validators.compose([Validators.required])],
      codigo: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(20)
        ]),
      ],
      descripcion: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(100)
        ]),
      ],
      tipoTransaccion: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
        ]),
      ],
      generica: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
        ]),
      ],
      subGenerica: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
        ]),
      ],
      subGenericaDetalle: [
        null,
        Validators.compose([Validators.pattern("^[0-9]+$")]),
      ],
      especifica: [null, Validators.compose([Validators.pattern("^[0-9]+$")])],
      especificaDetalle: [
        null,
        Validators.compose([Validators.pattern("^[0-9]+$")]),
      ],
      estado: [true],
      usuarioCreador: [data.usuarioCreador]
    });
  }
  ngOnInit() {
    this.cuentaContableService.getCuentaContables().subscribe((response) => {
      if (response.success) {
        this.cuentasContables = response.data;
      }
    });
  }

  onSubmit(form: ClasificadorIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }


    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let clasificadorIngreso = form;
    clasificadorIngreso.tipoTransaccion = +form.tipoTransaccion;
    clasificadorIngreso.subGenerica = +form.subGenerica;
    clasificadorIngreso.subGenericaDetalle = +form.subGenericaDetalle;
    clasificadorIngreso.especificaDetalle = +form.especificaDetalle;
    clasificadorIngreso.especifica = +form.especifica;
    clasificadorIngreso.especificaDetalle = +form.especificaDetalle;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.clasificadorIngresoService.createClasificadorIngreso(clasificadorIngreso).subscribe(
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
