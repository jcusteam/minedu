import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { CatalogoBien } from "src/app/core/models/catalogobien";
import { AuthService } from "src/app/core/services/auth.service";
import { CatalogoBienesService } from "src/app/core/services/catalogo-bienes.service";
import { ClasificadorIngresoService } from "src/app/core/services/clasificador-ingreso.service";
import { MessageService } from "src/app/core/services/message.service";
import { UnidadMedidaService } from "src/app/core/services/unidad-medida.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";


@Component({
  selector: "app-edit-catalogo-bien",
  templateUrl: "./edit-catalogo-bien.component.html",
  styleUrls: ["./edit-catalogo-bien.component.scss"],
})
export class EditCatalogoBienComponent implements OnInit {
  form: FormGroup;
  clasigficadorIngresos = [];
  unidadesMedidas = [];

  constructor(
    public dialogRef: MatDialogRef<EditCatalogoBienComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CatalogoBien,
    public fb: FormBuilder,
    private catalogoBienService: CatalogoBienesService,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private unidadMedidaService: UnidadMedidaService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      catalogoBienId: [data?.catalogoBienId],
      clasificadorIngresoId: [
        data?.clasificadorIngresoId,
        Validators.compose([Validators.required]),
      ],
      unidadMedidaId: [
        data?.unidadMedidaId,
        Validators.compose([Validators.required]),
      ],
      codigo: [
        data?.codigo,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(13)
        ]),
      ],
      descripcion: [
        data?.descripcion,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(200)
        ]),
      ],
      stockMaximo: [
        data?.stockMaximo,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
        ]),
      ],
      stockMinimo: [
        data?.stockMinimo,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
        ]),
      ],
      puntoReorden: [
        data?.puntoReorden,
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
        ]),
      ],
      estado: [data?.estado],
      usuarioModificador: [data?.usuarioModificador]
    });
  }
  ngOnInit() {
    this.clasificadorIngresoService
      .getClasificadorIngresos()
      .subscribe((response) => {
        if (response.success) {
          this.clasigficadorIngresos = response.data;
        }
      });
    this.unidadMedidaService.getUnidadMedidas().subscribe((response) => {
      if (response.success) {
        this.unidadesMedidas = response.data;
      }
    });
  }

  onSubmit(form: CatalogoBien) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let catalogoBien = form;
    catalogoBien.stockMaximo = +form.stockMaximo;
    catalogoBien.stockMinimo = +form.stockMinimo;
    catalogoBien.puntoReorden = +form.puntoReorden;
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Actualizando...");
        this.catalogoBienService.updateCatalogoBien(catalogoBien)
          .subscribe(
            (response) => {
              var message = response.messages.join(",");
              this.handleResponse(response.messageType, message, response.success);
            },
            (error) => this.handleError(error)
          );
      },
      () => { }
    );

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
