import { TransversalService } from 'src/app/core/services/transversal.service';
import { Combobox } from './../../../../../core/interfaces/combobox';
import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ClasificadorIngreso } from "src/app/core/models/clasificadoringreso";
import { GrupoRecaudacion } from "src/app/core/models/gruporecaudacion";
import { Tarifario } from "src/app/core/models/tarifario";
import { AuthService } from "src/app/core/services/auth.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { GrupoRecaudacionService } from "src/app/core/services/grupo-recaudacion.service";
import { MessageService } from "src/app/core/services/message.service";
import { TarifarioService } from "src/app/core/services/tarifario.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-new-tarifario",
  templateUrl: "./new-tarifario.component.html",
  styleUrls: ["./new-tarifario.component.scss"],
})
export class NewTarifarioComponent implements OnInit {
  form: FormGroup;

  clasigficadorIngresos: ClasificadorIngreso[] = [];
  grupoRecaudaciones: GrupoRecaudacion[] = [];

  preciosVariables: Combobox[] = [];

  constructor(
    public dialogRef: MatDialogRef<NewTarifarioComponent>,
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
      clasificadorIngresoId: [null, Validators.compose([Validators.required])],
      grupoRecaudacionId: [null, Validators.compose([Validators.required])],
      codigo: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(10)
        ]),
      ],
      nombre: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(300)
        ]),
      ],
      porcentajeUit: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^[.\d]+$/),
          Validators.maxLength(12)
        ]),
      ],
      precio: [
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^[.\d]+$/),
          Validators.maxLength(12)
        ]),
      ],
      precioVariable: [null,
        Validators.compose([Validators.required])],
      estado: [true],
      usuarioCreador: [data?.usuarioCreador]
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
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.tarifarioService.createTarifario(tarifario).subscribe(
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
