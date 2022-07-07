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

import { ClienteService } from "src/app/core/services/cliente.service";
import { NewRetencionComponent } from "../dialogs/new-retencion/new-retencion.component";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { Settings } from "src/app/app.settings.model";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { ComprobanteRetencion, ComprobanteRetencionFilter } from "src/app/core/models/comprobanteretencion";
import { AccionEnum, EstadoComprobanteRetencionEnum, FileServerEnum, MenuEnum, TipoComprobanteEnum, TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
import { ComprobanteRetencionService } from "src/app/core/services/comprobante-retencion.service";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { TYPE_MESSAGE } from "src/app/core/utils/messages";
import { InfoRetencionComponent } from "../dialogs/info-retencion/info-retencion.component";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { FileService } from "src/app/core/services/file.service";

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
  selector: "app-retencion",
  templateUrl: "./retencion.component.html",
  styleUrls: ["./retencion.component.scss"],
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
export class RetencionComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  // Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoRetencionEnum = EstadoComprobanteRetencionEnum;
  tipoComprobanteEnum = TipoComprobanteEnum;
  fileServerEnum = FileServerEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "fechaEmision",
    "periodo",
    "serie",
    "correlativo",
    "estado",
    "regimenRetencion",
    "nroDocumento",
    "nombreCliente",
    "total",
    "pdf",
    "actions",
  ];

  filter = new ComprobanteRetencionFilter();
  dataSource: ComprobanteRetencionsDataSource;

  //Tipo de documentos de identidad del cliente
  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];
  acciones: Accion[] = [];

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  // Fecha
  minDate = new Date();
  maxDate = new Date();

  constructor(
    public comprobanteRetencionService: ComprobanteRetencionService,
    private datePipe: DatePipe,
    public dialog: MatDialog,
    private clienteService: ClienteService,
    private fileService: FileService,
    private messageService: MessageService,
    private router: Router,
    private authService: AuthService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
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
      tipoDocumentoIdentidadId: [this.tipoDocIdentidadEnum.RUC],
      numeroDocumento: [null, Validators.compose([
        Validators.pattern("^[0-9]{11,11}$"),
        Validators.maxLength(11)
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
    this.filter.sortColumn = "comprobanteRetencionId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;

    this.dataSource = new ComprobanteRetencionsDataSource(this.comprobanteRetencionService);
    this.onlaodAccion();
    this.dataSource.loadComprobanteRetencions(this.filter);


    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService.getTipoDocumentoIdentidades().subscribe((response) => {
      if (response.success) {
        this.tipoDocumentoClientes = response.data.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.RUC);
      }
    });

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadComprobanteRetencionPage()))
      .subscribe();
  }

  loadComprobanteRetencionPage() {
    this.filter.sortColumn = "comprobanteRetencionId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadComprobanteRetencions(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.RETENCION)
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

    this.loadComprobanteRetencionPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.form.reset();
    this.form.patchValue({
      tipoDocumentoIdentidadId: this.tipoDocIdentidadEnum.RUC,
      numeroDocumento: null
    });
    this.filter = new ComprobanteRetencionFilter();
    this.loadComprobanteRetencionPage();
  }

  onSearch(form: ComprobanteRetencionFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid)
      return;

    this.filter.serie = form.serie;
    this.filter.correlativo = form.correlativo;
    this.filter.clienteId = form.clienteId;
    if (form.fechaInicio != null && form.fechaFin != null) {
      this.filter.fechaInicio = this.datePipe.transform(form.fechaInicio, "yyyy-MM-dd");
      this.filter.fechaFin = this.datePipe.transform(form.fechaFin, "yyyy-MM-dd");
    }

    this.loadComprobanteRetencionPage();
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
    let comprobanteRetencion = new ComprobanteRetencion();
    comprobanteRetencion.unidadEjecutoraId = +this.settings.unidadEjecutora;
    comprobanteRetencion.tipoComprobanteId = this.tipoComprobanteEnum.COMPROBANTE_RETENCION;
    comprobanteRetencion.tipoDocumentoId = this.tipoDocEnum.COMPROBANTE_RETENCION;
    comprobanteRetencion.estado = this.estadoRetencionEnum.EMITIDO;
    comprobanteRetencion.usuarioCreador = this.usuario.numeroDocumento;

    const dialogRef = this.dialog.open(NewRetencionComponent, {
      width: "1000px",
      data: comprobanteRetencion,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(data: ComprobanteRetencion): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    const dialogRef = this.dialog.open(InfoRetencionComponent, {
      width: "1000px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onPdf(comprobante: ComprobanteRetencion) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

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

export class ComprobanteRetencionsDataSource
  implements DataSource<ComprobanteRetencion>
{
  private ComprobanteRetencionsSubject = new BehaviorSubject<
    ComprobanteRetencion[]
  >([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;
  constructor(
    private comprobanteRetencionService: ComprobanteRetencionService
  ) { }

  loadComprobanteRetencions(filter: ComprobanteRetencionFilter) {
    this.loadingSubject.next(true);
    this.comprobanteRetencionService
      .getComprobanteRetencionesFilter(filter)
      .subscribe(
        (response) => {
          if (response.success) {
            setTimeout(() => {
              response.data.items.forEach(
                (item: ComprobanteRetencion, index) => {
                  item.index =
                    (filter.pageNumber - 1) * filter.pageSize + (index + 1);
                }
              );
              this.ComprobanteRetencionsSubject.next(response.data.items);
              this.totalItems = response.data.total;
              this.isLoadingResults = false;
            }, 500);
          } else {
            this.isLoadingResults = false;
          }
        },
        () => {
          this.isLoadingResults = false;
        }
      );
  }

  connect(
    collectionViewer: CollectionViewer
  ): Observable<ComprobanteRetencion[]> {
    return this.ComprobanteRetencionsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.ComprobanteRetencionsSubject.complete();
    this.loadingSubject.complete();
  }
}
