import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { PeriodoUitEnum } from "src/app/core/enums/recaudacion.enum";
import { Uit } from "src/app/core/models/uit";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { UitService } from "src/app/core/services/uit.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-new-uit",
  templateUrl: "./new-uit.component.html",
  styleUrls: ["./new-uit.component.scss"],
})
export class NewUitComponent implements OnInit {
  form: FormGroup;
  periodoUitEnum = PeriodoUitEnum;
  periodoMin = this.periodoUitEnum.ANIO_INICIO;
  periodoMax = new Date().getFullYear();
  constructor(
    public dialogRef: MatDialogRef<NewUitComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Uit,
    public fb: FormBuilder,
    private uitService: UitService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      periodo: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.min(this.periodoMin),
          Validators.max(this.periodoMax),
          Validators.maxLength(4)
        ]),
      ],
      unidadMonetaria: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(12)
        ]),
      ],
      valor: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^[.\d]+$/),
        ]),
      ],
      porcentaje: [0],
      baseLegal: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(100)
        ]),
      ],
      estado: [true],
      usuarioCreador: [data?.usuarioCreador]
    });
  }
  ngOnInit() { }

  onSubmit(form: Uit) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let uit = form;
    uit.fechaRegistro = new Date();
    uit.valor = +uit.valor;
    uit.porcentaje = +uit.porcentaje;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.uitService.createUit(uit).subscribe(
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
