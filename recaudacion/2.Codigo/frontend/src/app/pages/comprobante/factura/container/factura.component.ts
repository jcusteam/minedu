import { Component, OnInit, ViewChild, AfterViewInit, DEFAULT_CURRENCY_CODE } from "@angular/core";
import { DatePipe } from "@angular/common";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { Router } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import { DataSource } from "@angular/cdk/table";
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from "@angular/material-moment-adapter";
import { MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS, } from "@angular/material/core";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import * as FileSaver from "file-saver";

import { AppSettings } from "src/app/app.settings";
import { TipoDocEnum, TipoDocIdentidadEnum, EstadoComprobantePagoEnum, TipoComprobanteEnum, TipoCaptacionEnum, TipoAdquisicionEnum, TipoOperacionEnum, FileServerEnum, MenuEnum, AccionEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { Cliente } from "src/app/core/models/cliente";
import { ComprobantePagoFilter, ComprobantePago, ComprobantePagoEstado } from "src/app/core/models/comprobantepago";
import { TipoCaptacion } from "src/app/core/models/tipocaptacion";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { AuthService } from "src/app/core/services/auth.service";
import { ClienteService } from "src/app/core/services/cliente.service";
import { ComprobantePagoService } from "src/app/core/services/comprobante-pago.service";
import { FileService } from "src/app/core/services/file.service";
import { MessageService } from "src/app/core/services/message.service";
import { TipoCaptacionService } from "src/app/core/services/tipo-captacion.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { TransversalService } from "src/app/core/services/transversal.service";
import { TYPE_MESSAGE, MESSAGES } from "src/app/core/utils/messages";
import { InfoFacturaComponent } from "../dialogs/info-factura/info-factura.component";
import { NewFacturaComponent } from "../dialogs/new-factura/new-factura.component";
import { Settings } from "src/app/app.settings.model";



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
  selector: 'app-factura',
  templateUrl: './factura.component.html',
  styleUrls: ['./factura.component.scss'],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
    { provide: DEFAULT_CURRENCY_CODE, useValue: "PEN" },
  ],
})
export class FacturaComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoComprobantePagoEnum = EstadoComprobantePagoEnum;
  tipoComprobanteEnum = TipoComprobanteEnum;
  tipoCaptacionEnum = TipoCaptacionEnum;
  tipoAdquisicionEnum = TipoAdquisicionEnum;
  tipoOperacionEnum = TipoOperacionEnum;
  fileServerEnum = FileServerEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "fechaEmision",
    "serie",
    "correlativo",
    "estado",
    "tipoDocumento",
    "nroDocumento",
    "nombreCliente",
    "total",
    "pdf",
    "actions",
  ];

  filter = new ComprobantePagoFilter();
  dataSource: ComprobantePagoDataSource;

  clientes: Cliente[] = [];
  tipoCaptaciones: TipoCaptacion[] = [];
  tipoAdquisiciones: Combobox[] = [];
  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];
  acciones: Accion[] = [];

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  // Fecha
  minDate = new Date();
  maxDate = new Date();

  constructor(
    private comprobantePagoService: ComprobantePagoService,
    private datePipe: DatePipe,
    public dialog: MatDialog,
    private clienteService: ClienteService,
    private fileService: FileService,
    private tipoCaptacionService: TipoCaptacionService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private transversalService: TransversalService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public appSettings: AppSettings,
    public fb: FormBuilder,
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      serie: [null,
        Validators.compose([
          Validators.pattern("^[a-zA-Z0-9]+$"),
          Validators.maxLength(6)
        ])
      ],
      correlativo: [null,
        Validators.compose([
          Validators.pattern("^[0-9]+$"),
          Validators.maxLength(12)
        ])
      ],
      fechaInicio: [null],
      fechaFin: [null],
      tipoCaptacionId: [null],
      tipoAdquisicion: [null],
      tipoDocumentoIdentidadId: [null],
      numeroDocumento: [null, Validators.compose([
        Validators.pattern("^[a-zA-Z0-9]+$"),
        Validators.maxLength(12)
      ])],
      clienteId: [null],
      clienteNombre: [null],
      estado: [null]
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "comprobantePagoId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.filter.tipoDocumentoId = this.tipoDocEnum.FACTURA;

    this.dataSource = new ComprobantePagoDataSource(this.comprobantePagoService);
    this.onlaodAccion();
    this.dataSource.loadComprobantePagos(this.filter);
    this.loadMaestras();

  }

  loadMaestras() {
    this.tipoCaptacionService.getTipoCaptaciones().subscribe((response) => {
      if (response.success) {
        this.tipoCaptaciones = response.data.filter(x => x.tipoCaptacionId != this.tipoCaptacionEnum.VARIOS);
      }
    });

    this.transversalService.getTipoAdquisiciones().subscribe((response) => {
      if (response.success) {
        this.tipoAdquisiciones = response.data;
      }
    });

    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService.getTipoDocumentoIdentidades().subscribe((response) => {
      if (response.success) {
        let tipoDocsFactura = response.data;
        let tipoDocsBoleta = response.data;

        switch (this.filter.tipoDocumentoId) {
          case this.tipoDocEnum.FACTURA:
            this.tipoDocumentoClientes = tipoDocsFactura.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.RUC);
            break;
          case this.tipoDocEnum.BOLETA_VENTA:
            this.tipoDocumentoClientes = tipoDocsBoleta.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI || x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE);
            break;
          case this.tipoDocEnum.NOTA_CREDITO:
            this.tipoDocumentoClientes = response.data;
            break;
          case this.tipoDocEnum.NOTA_DEBITO:
            this.tipoDocumentoClientes = response.data;
            break;
          default:
            break;
        }
      }
    });

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadComprobantePagoPage()))
      .subscribe();
  }

  loadComprobantePagoPage() {
    this.filter.sortColumn = "comprobantePagoId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.filter.tipoDocumentoId = this.tipoDocEnum.FACTURA;
    this.dataSource.loadComprobantePagos(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.FACTURA)
      .subscribe(
        (response) => {
          if (response.success) {
            this.acciones = response.data;
            this.onAcceder(this.accionEnum.ACCEDER);
          }
          else {
            this.navigateIndex();
          }
        },
        (error) => {
          this.handleError(error)
        });
  }

  onAcceder(nombreAccion) {
    if (this.acciones.filter(x => x.nombrePermiso == nombreAccion).length == 0) {
      this.navigateIndex();
    }
  }

  onAccion(nombreAccion) {
    return this.acciones.filter(x => x.nombrePermiso == nombreAccion).length > 0;
  }
  onRefresh() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.loadComprobantePagoPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.form.reset();
    this.filter = new ComprobantePagoFilter();
    this.form.get("numeroDocumento").clearValidators();
    this.form.get("numeroDocumento").updateValueAndValidity();

    this.loadComprobantePagoPage();
  }

  onSearch(form: ComprobantePagoFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid)
      return;

    this.filter.serie = form.serie;
    this.filter.correlativo = form.correlativo;
    this.filter.tipoAdquisicion = form.tipoAdquisicion;
    this.filter.tipoCaptacionId = form.tipoCaptacionId;
    this.filter.clienteId = form.clienteId;

    if (form.fechaInicio != null && form.fechaFin != null) {
      this.filter.fechaInicio = this.datePipe.transform(form.fechaInicio, "yyyy-MM-dd");
      this.filter.fechaFin = this.datePipe.transform(form.fechaFin, "yyyy-MM-dd");
    }
    this.loadComprobantePagoPage();
  }

  // Cliente
  selectedChandgeTipoDocIdentidad(id: number) {
    this.form.patchValue({
      numeroDocumento: null,
      clienteNombre: null,
    });

    this.valiationTipoDocIdentidad(id);
  }

  valiationTipoDocIdentidad(id: number) {
    if (id == this.tipoDocIdentidadEnum.DNI) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{8,8}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.CE) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.RUC) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{11,11}$"),
        ]);
      this.form.updateValueAndValidity();
    } else {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    }
  }

  searchNroDoc() {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.form.patchValue({
      clienteNombre: null,
    });

    const tipoDocIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;
    const numeroDocumento = this.form.get("numeroDocumento").value;

    if (isNaN(tipoDocIdentidadId)) {
      return;
    }

    if (!this.form.get("numeroDocumento").valid || numeroDocumento == null) {
      return;
    }

    this.clienteService.getClienteByTipoNroDocumento(tipoDocIdentidadId, numeroDocumento).subscribe(
      (response) => {
        var message = response.messages.join(",");
        if (response.success) {
          let cliente = response.data;
          this.form.patchValue({ clienteNombre: cliente.nombre, clienteId: cliente.clienteId })
        } else {
          if (response.messageType == TYPE_MESSAGE.INFO) {
            this.messageService.msgInfo(message, () => { });
          } else if (response.messageType == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          } else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => this.handleError(error)
    );
  }

  openDialogNew(): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let comprobantePago = new ComprobantePago();
    comprobantePago.unidadEjecutoraId = +this.settings.unidadEjecutora;
    comprobantePago.tipoDocumentoId = this.tipoDocEnum.FACTURA;
    comprobantePago.tipoComprobanteId = this.tipoComprobanteEnum.FACTURA;
    comprobantePago.estado = this.estadoComprobantePagoEnum.EMITIDO;
    comprobantePago.tipoAdquisicion = this.tipoAdquisicionEnum.SERVICIO;
    comprobantePago.codigoTipoOperacion = this.tipoOperacionEnum.VENTA_INTERNA;
    comprobantePago.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewFacturaComponent, {
      width: "1200px",
      disableClose: true,
      data: comprobantePago,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
  }

  openDialogInfo(data): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoFacturaComponent, {
      width: "1200px",
      data: data,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onPdf(comprobante: ComprobantePago) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (comprobante?.nombreArchivo == null)
      return;

    const fileName = comprobante?.nombreArchivo?.trim() + ".pdf";

    this.fileService.verifyExists(this.fileServerEnum.COMPROBANTE, fileName).subscribe(
      (response) => {
        if (response.success) {
          this.fileService.donwloadFile(this.fileServerEnum.COMPROBANTE, fileName).subscribe(
            (file: Blob) => {
              FileSaver.saveAs(file, fileName);
            },
            (error) => { this.handleError(error) }
          );
        }
        else {
          var message = response.messages.join(",");
          this.handleResponse(response.messageType, message, response.success);
        }
      },
      (error) => { this.handleError(error) }
    );


  }

  onXml(comprobante: ComprobantePago) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const fileName = comprobante.nombreArchivo.trim() + ".xml";
    this.fileService.donwloadFile("comprobantes", fileName).subscribe(
      (file: Blob) => {
        FileSaver.saveAs(file, fileName);
      },
      () => { }
    );
  }

  onRechazar(data: ComprobantePago) {
    this.messageService.msgConfirm("¿Desea anular la boleta de venta?",
      () => {
        this.messageService.msgLoad("Actualizando...");

        let comprobantePagoEstado = new ComprobantePagoEstado();
        comprobantePagoEstado.comprobantePagoId = data.comprobantePagoId;
        comprobantePagoEstado.estado = this.estadoComprobantePagoEnum.RECHAZADA;
        comprobantePagoEstado.usuarioModificador = this.usuario.numeroDocumento;

        this.comprobantePagoService.updateEstadoComprobantePago(comprobantePagoEstado).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      },
      () => {
        this.onRefresh();
      }
    );
  }

  handleResponse(type, message, success) {
    if (success) {
      this.messageService.msgSuccess(message, () => {
        this.onRefresh();
      });
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { this.onRefresh(); });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { this.onRefresh(); });
      }
      else {
        this.messageService.msgError(message, () => { this.onRefresh(); });
      }
    }
  }

  handleError(error) {
    throw error;
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  navigateIndex() {
    this.router.navigateByUrl("/");
  }

}

export class ComprobantePagoDataSource implements DataSource<ComprobantePago> {
  private ComprobantePagosSubject = new BehaviorSubject<ComprobantePago[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private ComprobantePagoService: ComprobantePagoService) { }

  loadComprobantePagos(filter: ComprobantePagoFilter) {
    this.loadingSubject.next(true);
    this.ComprobantePagoService.getComprobantePagosFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          response.data.items.forEach((item: ComprobantePago, index) => {
            item.index =
              (filter.pageNumber - 1) * filter.pageSize + (index + 1);
          });
          this.ComprobantePagosSubject.next(response.data.items);
          this.totalItems = response.data.total;
          this.isLoadingResults = false;
        } else {
          this.isLoadingResults = false;
        }
      },
      () => {
        this.isLoadingResults = false;
      }
    );
  }

  connect(collectionViewer: CollectionViewer): Observable<ComprobantePago[]> {
    return this.ComprobantePagosSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.ComprobantePagosSubject.complete();
    this.loadingSubject.complete();
  }
}
