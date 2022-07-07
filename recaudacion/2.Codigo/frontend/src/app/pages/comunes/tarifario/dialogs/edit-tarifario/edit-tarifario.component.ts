import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Combobox } from "src/app/core/interfaces/combobox";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import { GrupoRecaudacion } from "src/app/core/models/gruporecaudacion";
import { Tarifario } from "src/app/core/models/tarifario";
import { AuthService } from "src/app/core/services/auth.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { GrupoRecaudacionService } from "src/app/core/services/grupo-recaudacion.service";
import { MessageService } from "src/app/core/services/message.service";
import { TarifarioService } from "src/app/core/services/tarifario.service";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-edit-tarifario",
  templateUrl: "./edit-tarifario.component.html",
  styleUrls: ["./edit-tarifario.component.scss"],
})
export class EditTarifarioComponent implements OnInit {
  form: FormGroup;

  clasigficadorIngresos: ClasificadorIngreso[] = [];
  grupoRecaudaciones: GrupoRecaudacion[] = [];

  preciosVariables: Combobox[] = [];

  constructor(
    public dialogRef: MatDialogRef<EditTarifarioComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Tarifario,
    public fb: FormBuilder,
    private tarifarioService: TarifarioService,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private grupoRecaudacionService: GrupoRecaudacionService,
    private transversalService: TransversalService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      tarifarioId: [data?.tarifarioId],
      clasificadorIngresoId: [
        data?.clasificadorIngresoId,
        Validators.compose([Validators.required])
      ],
      grupoRecaudacionId: [
        data?.grupoRecaudacionId,
        Validators.compose([Validators.required])
      ],
      codigo: [
        data?.codigo,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10)
        ]),
      ],
      nombre: [
        data?.nombre,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(300)
        ]),
      ],
      porcentajeUit: [
        data?.porcentajeUit,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^[.\d]+$/),
          Validators.maxLength(12)
        ]),
      ],
      precio: [
        data?.precio,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^[.\d]+$/),
          Validators.maxLength(12)
        ]),
      ],
      precioVariable: [data?.precioVariable,
      Validators.compose([Validators.required])],
      estado: [data?.estado],
      usuarioModificador: [data?.usuarioModificador]
    });
  }

  ngOnInit() {
    this.clasificadorIngresoService.getClasificadorIngresos().subscribe((response) => {
      if (response.success) {
        this.clasigficadorIngresos = response.data;
      }
    });

    this.grupoRecaudacionService.getGrupoRecaudacions().subscribe((response) => {
      if (response.success) {
        this.grupoRecaudaciones = response.data;
      }
    });

    this.transversalService.getPreciosVariables().subscribe((response) => {
      if (response.success) {
        this.preciosVariables = response.data;
      }
    });
  }

  onSubmit(form: Tarifario) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }
    let tarifario = form;
    tarifario.precio = +form.precio;
    tarifario.porcentajeUit = +form.porcentajeUit;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.tarifarioService.updateTarifario(tarifario).subscribe(
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
