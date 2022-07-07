import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ClasificadorIngreso } from 'src/app/core/models/clasificadoringreso';
import { CuentaContable } from 'src/app/core/models/cuentacontable';
import { AuthService } from 'src/app/core/services/auth.service';
import { ClasificadorIngresoService } from 'src/app/core/services/clasificador-ingreso.service';
import { CuentaContableService } from 'src/app/core/services/cuenta-contable.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-edit-clasificador-ingreso',
  templateUrl: './edit-clasificador-ingreso.component.html',
  styleUrls: ['./edit-clasificador-ingreso.component.scss']
})
export class EditClasificadorIngresoComponent implements OnInit {
  form: FormGroup;
  cuentasContables:CuentaContable [] = [];

  constructor(
    public dialogRef: MatDialogRef<EditClasificadorIngresoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ClasificadorIngreso,
    public fb: FormBuilder,
    private clasificadorIngresoService: ClasificadorIngresoService,
    private cuentaContableService: CuentaContableService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      clasificadorIngresoId:[data.clasificadorIngresoId],
      cuentaContableIdDebe: [data?.cuentaContableIdDebe, Validators.compose([Validators.required])],
      cuentaContableIdHaber: [data?.cuentaContableIdHaber, Validators.compose([Validators.required])],
      codigo: [data?.codigo, 
        Validators.compose([
          Validators.required,
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(20)
        ])
      ],
      descripcion: [data?.descripcion, 
        Validators.compose([
          Validators.required, 
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(100)
        ]
      )],
      tipoTransaccion: [data?.tipoTransaccion, Validators.compose([Validators.required, Validators.required,Validators.pattern('^[0-9]+$')])],
      generica: [data?.generica, Validators.compose([Validators.required, Validators.pattern('^[0-9]+$')])],
      subGenerica: [data?.subGenerica, Validators.compose([Validators.required, Validators.pattern('^[0-9]+$')])],
      subGenericaDetalle: [data?.subGenericaDetalle, Validators.compose([Validators.pattern('^[0-9]+$')])],
      especifica: [data?.especifica, Validators.compose([Validators.pattern('^[0-9]+$')])],
      especificaDetalle: [data?.especificaDetalle, Validators.compose([Validators.pattern('^[0-9]+$')])],
      estado: [data?.estado],
      usuarioModificador:[data?.usuarioModificador]
    });
  }
  ngOnInit() {
    
    this.cuentaContableService.getCuentaContables().subscribe(
      (response) => {
        if (response.success) {
          this.cuentasContables = response.data;
        }
      }
    );
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

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_UPDATE,
      () => {
        this.messageService.msgLoad("Guardando...");
        this.clasificadorIngresoService.updateClasificadorIngreso(clasificadorIngreso).subscribe(
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
