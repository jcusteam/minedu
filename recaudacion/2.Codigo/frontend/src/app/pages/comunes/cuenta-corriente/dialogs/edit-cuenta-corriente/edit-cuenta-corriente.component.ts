import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Banco } from "src/app/core/models/banco";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { FuenteFinanciamiento } from "src/app/core/models/fuentefinanciamiento";
import { AuthService } from "src/app/core/services/auth.service";
import { BancoService } from "src/app/core/services/banco.service";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { MessageService } from "src/app/core/services/message.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-edit-cuenta-corriente",
  templateUrl: "./edit-cuenta-corriente.component.html",
  styleUrls: ["./edit-cuenta-corriente.component.scss"],
})
export class EditCuentaCorrienteComponent implements OnInit {
  form: FormGroup;

  bancos: Banco[] = [];
  fuenteFinanciamientos: FuenteFinanciamiento[] = [];

  constructor(
    public dialogRef: MatDialogRef<EditCuentaCorrienteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CuentaCorriente,
    public fb: FormBuilder,
    private cuentaCorrienteService: CuentaCorrienteService,
    private bancoService: BancoService,
    private fuenteFinanciamientoService: FuenteFinanciamientoService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      cuentaCorrienteId: [data.cuentaCorrienteId],
      bancoId: [data.bancoId, Validators.compose([Validators.required])],
      fuenteFinanciamientoId: [data?.fuenteFinanciamientoId,
        Validators.compose([Validators.required]),
      ],
      unidadEjecutoraId: [data?.unidadEjecutoraId,
        Validators.compose([Validators.required]),
      ],
      codigo: [
        data?.codigo,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(12)
        ]),
      ],
      numero: [
        data?.numero,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(30)
        ]),
      ],
      denominacion: [
        data?.denominacion,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(150)
        ]),
      ],
      fecha:[data?.fecha],
      estado: [data?.estado],
      usuarioModificador:[data?.usuarioModificador]
    });
  }
  ngOnInit() {
    this.bancoService.getBancos().subscribe((response) => {
      if (response.success) {
        this.bancos = response.data;
      }
    });
    this.fuenteFinanciamientoService.getFuenteFinanciamientos().subscribe((response) => {
      if (response.success) {
        this.fuenteFinanciamientos = response.data;
      }
    });
  }

  onSubmit(form: CuentaCorriente) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let cuentaCorriente = form;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.cuentaCorrienteService.updateCuentaCorriente(cuentaCorriente).subscribe(
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
