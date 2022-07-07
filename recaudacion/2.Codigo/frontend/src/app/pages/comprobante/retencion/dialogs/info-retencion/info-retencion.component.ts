import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";
import { DatePipe } from "@angular/common";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, } from "@angular/material/core";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from "@angular/material-moment-adapter";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, } from "@angular/material/dialog";
import { MatTableDataSource } from "@angular/material/table";
import { ComprobanteRetencionService } from 'src/app/core/services/comprobante-retencion.service';
import { ComprobanteRetencion, ComprobanteRetencionDetalle } from 'src/app/core/models/comprobanteretencion';
import { TipoDocIdentidadEnum, TipoRegimenRetencionEnum, ValorRegimenRetencionEnum } from 'src/app/core/enums/recaudacion.enum';
import { MessageService } from 'src/app/core/services/message.service';
import { Tools } from "src/app/core/utils/tools";
import { TransversalService } from "src/app/core/services/transversal.service";
import { Combobox } from 'src/app/core/interfaces/combobox';

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
  selector: 'app-info-retencion',
  templateUrl: './info-retencion.component.html',
  styleUrls: ['./info-retencion.component.scss'],
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
export class InfoRetencionComponent implements OnInit {


  form: FormGroup;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoRegimenRetencionEnum = TipoRegimenRetencionEnum;
  valorRegimenRetencionEnum = ValorRegimenRetencionEnum;

  tipoRegimenes: Combobox[] = [];

  comprobanteRetencionDetalles: ComprobanteRetencionDetalle[] = [];

  displayedColumns: string[] = [
    'nro', 'tipoDocumento', 'serie', 'numero', 'fechaEmision', 'total', 'nroPago', 'importePago', 'tasa', 'retencion', 'importeNetoPagado'];

  dataSource = new MatTableDataSource(this.comprobanteRetencionDetalles);

  maxDate = new Date();
  minDate = new Date();

  constructor(
    public dialogRef: MatDialogRef<InfoRetencionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobanteRetencion,
    private fb: FormBuilder,
    private datePipe: DatePipe,
    public dialog: MatDialog,
    private comprobanteRetencionService: ComprobanteRetencionService,
    private transversalService: TransversalService,
    private messageService: MessageService) {
    this.form = this.fb.group(
      {
        comprobanteRetencionId: [0],
        clienteId: [0],
        unidadEjecutoraId: [data?.unidadEjecutoraId],
        tipoDocumentoId: [data?.tipoComprobanteId],
        tipoComprobanteId: [data?.tipoComprobanteId],
        serie: [data?.serie],
        correlativo: [data?.correlativo],
        fechaEmision: [data?.fechaEmision],
        periodo: [data?.periodo],
        regimenRetencion: [data?.regimenRetencion],
        total: [data?.total?.toFixed(2)],
        nombreArchivo: [data?.nombreArchivo],
        observacion: [data?.observacion],
        estadoSunat: [data?.estadoSunat],
        estado: [data?.estado],

        // Extra
        numeroDocumento: [data?.cliente?.numeroDocumento],
        nombreCliente: [data?.cliente?.nombre],
        periodoOps: [this.datePipe.transform(data?.periodo, 'MM/yyyy')]
      });
  }

  ngOnInit() {

    this.loadData();

    // tipo regimenes
    this.transversalService.getTipoRegimenRetenciones().subscribe((response) => {
      if (response.success) {
        this.tipoRegimenes = response.data;
      }
    });
  }

  loadData() {
    this.comprobanteRetencionService.getComprobanteRetencionById(this.data.comprobanteRetencionId).subscribe(
      response => {
        if (response.success) {
          this.dataSource.data = response.data.comprobanteRetencionDetalle;
          this.dataSource._updateChangeSubscription();
        }
      }
    )
  }


  getTotal(): number {
    return this.comprobanteRetencionDetalles.map(t => t.importeNetoPagado).reduce((acc, value) => acc + value, 0);
  }

  onSubmit() {

  }

  handleError(error) {
    throw error;
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onNoClick(): void {
    this.dialogRef.close(0);
  }

  formatMoney(i) {
    return Tools.formatMoney(i);
  }



}
