import { LiquidacionEstado } from './../../../../core/models/liquidacion';
import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { MatDialog } from "@angular/material/dialog";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { CollectionViewer } from "@angular/cdk/collections";
import { finalize, tap } from "rxjs/operators";
import { Estado } from "src/app/core/models/estado";
import { Liquidacion, LiquidacionFilter, } from "src/app/core/models/liquidacion";
import { LiquidacionService } from "src/app/core/services/liquidacion.service";
import { EstadoService } from "src/app/core/services/estado.service";
import { NewLiquidacionIngresoComponent } from "../dialogs/new-liquidacion-ingreso/new-liquidacion-ingreso.component";
import { InfoLiquidacionIngresoComponent } from "../dialogs/info-liquidacion-ingreso/info-liquidacion-ingreso.component";
import { EditLiquidacionIngresoComponent } from "../dialogs/edit-liquidacion-ingreso/edit-liquidacion-ingreso.component";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoLiquidacionEnum, MenuEnum, TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-liquidacion-ingreso",
  templateUrl: "./liquidacion-ingreso.component.html",
  styleUrls: ["./liquidacion-ingreso.component.scss"],
})
export class LiquidacionIngresoComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enums 
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoLiquidacionEnum = EstadoLiquidacionEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "numero",
    "estado",
    "fechaRegistro",
    "fuenteFinanciamiento",
    "procedencia",
    "total",
    "actions",
  ];

  filter = new LiquidacionFilter();
  dataSource: LiquidacionesDataSource;

  estados: Estado[] = [];
  acciones: Accion[] = [];

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;


  constructor(
    public fb: FormBuilder,
    private liquidacionService: LiquidacionService,
    private estadoService: EstadoService,
    private dialog: MatDialog,
    private messageService: MessageService,
    private router: Router,
    private authService: AuthService,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      numero: [null],
      estado: [null]
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "liquidacionId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource = new LiquidacionesDataSource(this.liquidacionService);
    this.onlaodAccion();
    this.dataSource.loadLiquidaciones(this.filter);

    this.loadMaestras();
    
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadLiquidacionPage()))
      .subscribe();
  }

  loadMaestras() {
    // Listado de estados
    this.estadoService
      .getEstadoByTipoDocumento(this.tipoDocEnum.LIGUIDACION_RECAUDACION)
      .subscribe((response) => {
        if (response.success) {
          this.estados = response.data;
        }
      });
  }

  loadLiquidacionPage() {
    this.filter.sortColumn = "liquidacionId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadLiquidaciones(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.LIQUIDACION_INGRESO)
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

  onSearch(form: LiquidacionFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter.numero = form.numero;
    this.filter.estado = form.estado;

    this.loadLiquidacionPage();
  }

  onRefresh() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.loadLiquidacionPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter = new LiquidacionFilter();
    this.form.reset();
    this.loadLiquidacionPage();
  }

  // Nuevo
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let liquidacion = new Liquidacion();
    liquidacion.unidadEjecutoraId = +this.settings.unidadEjecutora;
    liquidacion.tipoDocumentoId = this.tipoDocEnum.LIGUIDACION_RECAUDACION;
    liquidacion.estado = this.estadoLiquidacionEnum.EMITIDO;
    liquidacion.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewLiquidacionIngresoComponent, {
      width: "1000px",
      disableClose: true,
      data: liquidacion,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Info
  openDialogInfo(liquidacion: Liquidacion): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoLiquidacionIngresoComponent, {
      width: "1000px",
      disableClose: true,
      data: liquidacion,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Edit
  openDialogEdit(liquidacion: Liquidacion): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    liquidacion.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditLiquidacionIngresoComponent, {
      width: "1000px",
      disableClose: true,
      data: liquidacion,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onProcess(data: Liquidacion) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_PROCESAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let liquidacionEstado= new LiquidacionEstado();
        liquidacionEstado.liquidacionId = data.liquidacionId;
        liquidacionEstado.estado = this.estadoLiquidacionEnum.PROCESADO;
        liquidacionEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.liquidacionService.updateEstadoLiquidacion(liquidacionEstado).subscribe(
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

  onEmitirRI(data: Liquidacion) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.LIQUIDACION.CONFIRM_EMITIR_RECIBO_INGRESO,
      () => {
        this.messageService.msgLoad("Emitiendo Recibo de ingreso...");
        let liquidacionEstado= new LiquidacionEstado();
        liquidacionEstado.liquidacionId = data.liquidacionId;
        liquidacionEstado.estado = this.estadoLiquidacionEnum.EMITIDO_RI;
        liquidacionEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.liquidacionService.updateEstadoLiquidacion(liquidacionEstado).subscribe(
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


  onDelete(data: Liquidacion) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminando...");
        this.liquidacionService.deleteLiquidacion(data.liquidacionId).subscribe(
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

  // Response
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

export class LiquidacionesDataSource implements DataSource<Liquidacion> {
  private _dataChange = new BehaviorSubject<any>([]);
  private _loadingChange = new BehaviorSubject<boolean>(false);
  public total = 0;
  public loading = this._loadingChange.asObservable();

  constructor(private liquidacionService: LiquidacionService) { }

  loadLiquidaciones(filter: LiquidacionFilter) {
    this._loadingChange.next(true);
    this.liquidacionService.getLiquidacionesFilter(filter)
      .pipe(
        finalize(() => this._loadingChange.next(false))
      )
      .subscribe(
        (response) => {
          if (response.success) {
            response.data.items.forEach((item: Liquidacion, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this._dataChange.next(response.data.items);
            this.total = response.data.total;
          } else {
            this._dataChange.next([]);
          }
        },
        () => {
          this._dataChange.next([]);
        }
      );
  }

  connect(collectionViewer: CollectionViewer): Observable<[]> {
    return this._dataChange.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this._dataChange.complete();
    this._loadingChange.complete();
  }

  get data(): any {
    return this._dataChange.value || [];
  }
}
