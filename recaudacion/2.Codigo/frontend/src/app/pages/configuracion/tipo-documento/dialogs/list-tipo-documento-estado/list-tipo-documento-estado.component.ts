import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

import { Estado } from 'src/app/core/models/estado';
import { TipoDocumento } from 'src/app/core/models/tipodocumento';
import { AuthService } from 'src/app/core/services/auth.service';
import { EstadoService } from 'src/app/core/services/estado.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';
import { EditTipoDocumentoEstadoComponent } from '../edit-tipo-documento-estado/edit-tipo-documento-estado.component';


@Component({
  selector: 'app-list-tipo-documento-estado',
  templateUrl: './list-tipo-documento-estado.component.html',
  styleUrls: ['./list-tipo-documento-estado.component.scss']
})
export class ListTipoDocumentoEstadoComponent implements OnInit {
  form: FormGroup;
  estados: Estado[] = [];

  displayedColumns: string[] = ['numero', 'orden', 'nombre', 'tipoDocumento', 'actions'];
  dataSource = new MatTableDataSource(this.estados);

  constructor(
    public dialogRef: MatDialogRef<ListTipoDocumentoEstadoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoDocumento,
    public fb: FormBuilder,
    private estadoService: EstadoService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      numero: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.min(1),
          Validators.max(10),
        ])
      ],
      orden: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[0-9]+$'),
          Validators.min(1)
        ])
      ],
      nombre: [null,
        Validators.compose([
          Validators.required,
          Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
          Validators.maxLength(50)
        ])
      ],
    });
  }
  ngOnInit() {
    this.onLoad();
  }

  onLoad() {
    this.estadoService.getEstadoByTipoDocumento(this.data.tipoDocumentoId).subscribe(
      (response) => {
        if (response.success) {
          this.estados = response.data;
          this.dataSource.data = this.estados;
          this.dataSource._updateChangeSubscription();
        }
      }
    );
  }

  onRefresh(){
    this.onLoad();
  }

  onSubmit(form: Estado) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      this.messageService.msgWarning(MESSAGES.FORM.ERROR_VALIDATION, () => { });
      return;
    }

    let estado = form;
    estado.tipoDocumentoId = this.data.tipoDocumentoId;
    estado.numero = +form.numero;
    estado.orden = +form.orden;
    estado.usuarioCreador = this.data.usuarioModificador;

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_REGISTER,
      () => {
        this.messageService.msgLoad("Guardando...");

        this.estadoService.createEstado(estado).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {

      });

  }

  openDialogEdit(data: Estado): void {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.data.usuarioModificador;
    const dialogRef = this.dialog.open(EditTipoDocumentoEstadoComponent, {
      disableClose: true,
      width: '600px',
      data: data
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onLoad();
    });
  }

  deleteRowData(estado: Estado) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminando...");

        this.estadoService.deleteEstado(estado.estadoId).subscribe(
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
        this.onRefresh();
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
