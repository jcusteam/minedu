import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { DatePipe } from "@angular/common";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, } from "@angular/material/core";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from "@angular/material-moment-adapter";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, } from "@angular/material/dialog";
import { MatTableDataSource } from "@angular/material/table";

import { ClienteService } from 'src/app/core/services/cliente.service';
import { NewRetencionDetalleComponent } from '../new-retencion-detalle/new-retencion-detalle.component';
import { ComprobanteRetencionService } from 'src/app/core/services/comprobante-retencion.service';
import { ComprobanteRetencion, ComprobanteRetencionDetalle } from 'src/app/core/models/comprobanteretencion';
import { TipoDocIdentidadEnum, TipoRegimenRetencionEnum, ValorRegimenRetencionEnum } from 'src/app/core/enums/recaudacion.enum';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { Tools } from "src/app/core/utils/tools";
import { TransversalService } from "src/app/core/services/transversal.service";
import { Combobox } from 'src/app/core/interfaces/combobox';
import { Cliente } from "src/app/core/models/cliente";
import { NewRetencionClienteComponent } from "../new-retencion-cliente/new-retencion-cliente.component";

const DATE_MODE_FORMATS = {
  parse: {
    dateInput: "DD/MM/YYYY",
  },
  display: {
    dateInput: "DD/MM/YYYY",
    monthYearLabel: "MMM YYYY",
    dateA11yLabel: "LL",
    monthYearA11yLabel: "MMMM YYYY",
  },
};

@Component({
  selector: 'app-new-retencion',
  templateUrl: './new-retencion.component.html',
  styleUrls: ['./new-retencion.component.scss'],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
  ],
})
export class NewRetencionComponent implements OnInit {

  form: FormGroup;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoRegimenRetencionEnum = TipoRegimenRetencionEnum;
  valorRegimenRetencionEnum = ValorRegimenRetencionEnum;

  tipoRegimenes: Combobox[] = [];

  comprobanteRetencionDetalles: ComprobanteRetencionDetalle[] = [];

  displayedColumns: string[] = ['nro', 'tipoDocumento', 'serie', 'numero', 'fechaEmision', 'total', 'nroPago', 'importePago', 'tasa', 'retencion', 'importeNetoPagado', 'actions'];

  dataSource = new MatTableDataSource(this.comprobanteRetencionDetalles);

   // Fecha
   minDateEmision = new Date();
   minDate = new Date();
   maxDate = new Date();

  constructor(
    public dialogRef: MatDialogRef<NewRetencionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobanteRetencion,
    private fb: FormBuilder,
    private datePipe: DatePipe,
    public dialog: MatDialog,
    private comprobanteRetencionService: ComprobanteRetencionService,
    private transversalService: TransversalService,
    private clienteService: ClienteService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.form = this.fb.group(
      {
        comprobanteRetencionId: [0],
        clienteId: [0],
        unidadEjecutoraId: [data?.unidadEjecutoraId],
        tipoDocumentoId: [data?.tipoDocumentoId],
        tipoComprobanteId: [data?.tipoComprobanteId],
        serie: [null],
        correlativo: [null],
        fechaEmision: [new Date(),
        Validators.compose([
          Validators.required
        ])],
        periodo: [new Date(),
        Validators.compose([
          Validators.required
        ])
        ],
        regimenRetencion: [null,
          Validators.compose([
            Validators.required
          ])
        ],
        total: [0,
          Validators.compose([
            Validators.pattern(/^[.\d]+$/),
            Validators.min(0),
            Validators.maxLength(12)
          ])
        ],
        totalPago: [0,
          Validators.compose([
            Validators.pattern(/^[.\d]+$/),
            Validators.min(0),
            Validators.maxLength(12)
          ])
        ],
        porcentaje: [0,
          Validators.compose([
            Validators.pattern(/^[.\d]+$/),
            Validators.min(0),
            Validators.maxLength(12)
          ])
        ],
        nombreArchivo: [null],
        observacion: [null,
          Validators.compose([
            Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#/\n()""°:;.,_ -]+$'),
            Validators.maxLength(200)
          ])
        ],
        estadoSunat: [null],
        estado: [data?.estado],
        usuarioCreador: [data?.usuarioCreador],

        // Extra
        numeroDocumento: [null,
          Validators.compose([
            Validators.required,
            Validators.pattern("^[0-9]{11,11}$")
          ])
        ],
        nombreCliente: [null,
          Validators.compose([Validators.required])
        ],

        periodoOps: [
          this.datePipe.transform(new Date(), 'MM/yyyy'),
          Validators.compose([Validators.required])
        ]
      });
  }

  ngOnInit() {
    this.minDateEmision = Tools.addDays(new Date(), -2);
    // tipo regimenes
    this.transversalService.getTipoRegimenRetenciones().subscribe((response) => {
      if (response.success) {
        this.tipoRegimenes = response.data;
      }
    });

  }

  // key Up Nro Cliente
  onKeyUpNroDocCliente(event) {
    this.form.patchValue({
      nombreCliente: null,
      clienteId: 0,
    });
  }

  // Buscar cliente
  searchCliente() {

    if (!this.form.get("numeroDocumento").valid) {
      return;
    }

    const numeroDocumento = this.form.get("numeroDocumento").value;
    this.messageService.msgLoad("Consultando Cliente...");
    this.clienteService.getClienteByTipoNroDocumento(this.tipoDocIdentidadEnum.RUC, numeroDocumento)
      .subscribe(
        (response) => {
          var message = response.messages.join(",");
          this.messageService.msgAutoClose();
          if (response.success) {
            this.form.patchValue({ clienteId: response.data.clienteId, nombreCliente: response.data?.nombre })
          }
          else {
            let type = response.messageType;
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
        },
        (error) => {
          this.handleError(error);
        }
      );
  }

  openDialogDetalle(): void {
    let detalle = new ComprobanteRetencionDetalle();

    if (!this.form.get("regimenRetencion").valid) {
      return;
    }

    const regimenRetencion = this.form.get("regimenRetencion").value;
    const regimenRetencionDesc = this.tipoRegimenes.filter(x => x.value == regimenRetencion)[0].label;
    if (regimenRetencion == this.tipoRegimenRetencionEnum.TASA_03) {
      detalle.tasa = this.valorRegimenRetencionEnum.TASA_03;
      this.form.patchValue({ porcentaje: this.valorRegimenRetencionEnum.TASA_03 });
    }
    else {
      detalle.tasa = this.valorRegimenRetencionEnum.TASA_06;
      this.form.patchValue({ porcentaje: this.valorRegimenRetencionEnum.TASA_06 });
    }

    detalle.regimenRetencionDesc = regimenRetencionDesc;
    detalle.estado = "1";
    detalle.usuarioCreador = this.data.usuarioCreador;

    const dialogRef = this.dialog.open(NewRetencionDetalleComponent, {
      width: '800px',
      data: detalle
    });

    dialogRef.afterClosed().subscribe((response: ComprobanteRetencionDetalle) => {
      if (response != null) {
        this.addRowData(response);
      }
    });
  }

  addRowData(data: ComprobanteRetencionDetalle) {
    this.dataSource.data.push(data);
    this.dataSource._updateChangeSubscription();
    this.form.patchValue({ total: this.getTotalImporteRetenido()?.toFixed(2) });
    this.form.patchValue({ totalPago: this.getTotalPago()?.toFixed(2) });
  }

  deleteRowData(data) {
    this.dataSource.data = this.dataSource.data.filter(obj => obj !== data);
    this.dataSource._updateChangeSubscription();
    this.form.patchValue({ total: this.getTotalImporteRetenido()?.toFixed(2) });
  }

  getTotalImporteRetenido(): number {
    return this.dataSource.data.map(t => t.importeRetenido).reduce((acc, value) => acc + value, 0);
  }

  getTotalPago(): number {
    return this.dataSource.data.map(t => t.importeNetoPagado).reduce((acc, value) => acc + value, 0);
  }

  onSubmit(form: ComprobanteRetencion) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("Ingrese el detalle del comprobante de retención", () => { });
      return;
    }

    let comprobante = form;
    comprobante.total = +this.getTotalImporteRetenido();
    comprobante.totalPago = +this.getTotalPago();
    comprobante.porcentaje = +form.porcentaje;
    comprobante.comprobanteRetencionDetalle = this.dataSource.data;

    this.messageService.msgConfirm(MESSAGES.FORM.COMPROBANTE_RETENCION.GENERAR,
      () => {
        this.messageService.msgLoad("Generando Comprobante...");
        this.comprobanteRetencionService.createComprobanteRetencion(comprobante).subscribe(
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

  formatMoney(i) {
    return Tools.formatMoney(i);
  }

  // Nuevo Cliente
  openDialogNewCliente() {
    let cliente = new Cliente();
    cliente.usuarioCreador = this.data.usuarioCreador;
    const dialogRef = this.dialog.open(NewRetencionClienteComponent, {
      width: "800px",
      disableClose: true,
      data: cliente,
    });

    dialogRef.afterClosed().subscribe((response: Cliente) => {
      if (response?.clienteId > 0) {
        this.form.patchValue({
          clienteId: response?.clienteId,
          numeroDocumento: response?.numeroDocumento,
          nombreCliente: response?.nombre,
        });
      }
    });
  }

}
