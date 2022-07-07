import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { RegistroLinea, RegistroLineaEstado, RegistroLineaFilter } from "src/app/core/models/registrolinea";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { RegistroLineaService } from "src/app/core/services/registro-linea.service";
import { MatDialog } from "@angular/material/dialog";
import { InfoRegistroLineaComponent } from "../dialogs/info-registro-linea/info-registro-linea.component";
import { EditRegistroLineaComponent } from "../dialogs/edit-registro-linea/edit-registro-linea.component";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { CollectionViewer } from "@angular/cdk/collections";
import { finalize, tap } from "rxjs/operators";
import { TipoReciboIngreso } from "src/app/core/models/tiporeciboingreso";
import { TipoReciboIngresoService } from "src/app/core/services/tipo-recibo-ingreso.service";
import { ObsRegistroLineaComponent } from "../dialogs/obs-registro-linea/obs-registro-linea.component";
import { Estado } from "src/app/core/models/estado";
import { EstadoService } from "src/app/core/services/estado.service";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { ClienteService } from "src/app/core/services/cliente.service";
import { NewRegistroLineaComponent } from "../dialogs/new-registro-linea/new-registro-linea.component";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { Settings } from "src/app/app.settings.model";
import { AppSettings } from "src/app/app.settings";
import { Router } from "@angular/router";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AccionEnum, EstadoRegistroLineaEnum, MenuEnum, TipoDocEnum, TipoDocIdentidadEnum, TipoReciboIngresoEnum, ValidaDepositoEnum } from "src/app/core/enums/recaudacion.enum";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-registro-linea",
  templateUrl: "./registro-linea.component.html",
  styleUrls: ["./registro-linea.component.scss"],
})
export class RegistroLineaComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoRegLineaEnum = EstadoRegistroLineaEnum;
  validaDepositoEnum = ValidaDepositoEnum;
  tipoReciboIngresoEnum = TipoReciboIngresoEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;


  displayedColumns: string[] = [
    "index",
    "numero",
    "fechaRegistro",
    "estado",
    "tipoDocumentoIdentidad",
    "numeroDocumento",
    "nombreCliente",
    "correo",
    "tipoReciboIngreso",
    "banco",
    "cuentaCorriente",
    "numeroDeposito",
    "importeDeposito",
    "fechaDeposito",
    "validarDeposito",
    "expedienteESinad",
    "numeroOficio",
    "numeroComprobantePago",
    "expedienteSiaf",
    "numeroResolucion",
    "actions",
  ];

  filter = new RegistroLineaFilter();
  dataSource: RegistroLineasDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  tipoReciboIngresos: TipoReciboIngreso[] = [];
  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];
  estados: Estado[] = [];
  acciones: Accion[] = [];


  constructor(
    public fb: FormBuilder,
    private registroLineaService: RegistroLineaService,
    private tipoReciboIngresoService: TipoReciboIngresoService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private estadoService: EstadoService,
    private clienteService: ClienteService,
    private dialog: MatDialog,
    public router: Router,
    private messageService: MessageService,
    private authService: AuthService,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      tipoDocumentoIdentidadId: [null],
      tipoReciboIngresoId: [null],
      numeroDocumento: [null, Validators.compose([
        Validators.pattern("^[a-zA-Z0-9]+$"),
        Validators.maxLength(12)
      ])],
      clienteNombre: [null],
      clienteId: [null],
      estado: [null],
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "registroLineaId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    //this.filter.rol = this.usuario.roleRecaudacion;
    this.dataSource = new RegistroLineasDataSource(this.registroLineaService);
    this.onlaodAccion();
    this.dataSource.loadRegistroLineas(this.filter);
    this.onLoadMaestras();
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadRegistroLineaPage()))
      .subscribe();
  }

  onLoadMaestras() {
    this.tipoReciboIngresoService
      .getTipoReciboIngresos()
      .subscribe((response) => {
        this.tipoReciboIngresos = response.data.filter(
          (obj) =>
            obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.EJECUCION_CARTA_FIANAZA ||
            obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.DEVOLUCION_SALGO_VIATICO ||
            obj.tipoReciboIngresoId == this.tipoReciboIngresoEnum.DEPOSITO_INDEBIDO
        );
      });

    this.estadoService
      .getEstadoByTipoDocumento(this.tipoDocEnum.REGISTRO_LINEA)
      .subscribe((response) => {
        if (response.success) {
          this.estados = response.data;
        }
      });

    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService
      .getTipoDocumentoIdentidades()
      .subscribe((response) => {
        if (response.success) {
          this.tipoDocumentoClientes = response.data;
        }
      });

  }

  loadRegistroLineaPage() {
    this.filter.sortColumn = "registroLineaId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadRegistroLineas(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.REGISTRO_LINEA)
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

  onSearch(form: RegistroLineaFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter.tipoReciboIngresoId = form.tipoReciboIngresoId;
    this.filter.clienteId = form.clienteId;
    this.filter.estado = form.estado;
    this.loadRegistroLineaPage();
  }

  onRefresh() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.loadRegistroLineaPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.form.reset();
    this.filter = new RegistroLineaFilter();
    this.form.get("numeroDocumento").clearValidators();
    this.form.get("numeroDocumento").updateValueAndValidity();
    this.loadRegistroLineaPage();
  }

  onSelectionTipoDocIdentidad(id: number) {
    this.filter.tipoDocumentoIdentidadId = id;
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

  // Nuevo
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let registroLinea = new RegistroLinea();
    registroLinea.unidadEjecutoraId = +this.settings.unidadEjecutora;
    registroLinea.tipoDocumentoId = this.tipoDocEnum.REGISTRO_LINEA;
    registroLinea.estado = this.estadoRegLineaEnum.EMITIDO;
    registroLinea.validarDeposito = this.validaDepositoEnum.PENDIENTE;
    registroLinea.usuarioCreador = this.usuario.numeroDocumento;

    const dialogRef = this.dialog.open(NewRegistroLineaComponent, {
      width: "1000px",
      disableClose: true,
      data: registroLinea,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(data: RegistroLinea): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoRegistroLineaComponent, {
      width: "1200px",
      disableClose: true,
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: RegistroLinea): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditRegistroLineaComponent, {
      width: "1200px",
      disableClose: true,
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogObservar(data: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let registroLineaEstado = new RegistroLineaEstado();
    registroLineaEstado.registroLineaId = data.registroLineaId;
    registroLineaEstado.estado = this.estadoRegLineaEnum.OBSERVADO;
    registroLineaEstado.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(ObsRegistroLineaComponent, {
      width: "800px",
      disableClose: true,
      data: registroLineaEstado,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onProcess(data: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.REGISTRO_LINEA.CONFIRM_INICIAR_PROCESO,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let registroLineaEstado = new RegistroLineaEstado();
        registroLineaEstado.registroLineaId = data.registroLineaId;
        registroLineaEstado.estado = this.estadoRegLineaEnum.EN_PROCESO;
        registroLineaEstado.usuarioModificador = this.usuario.numeroDocumento;

        this.registroLineaService.updateEstadoRegistroLinea(registroLineaEstado).subscribe(
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

  onDerivar(data: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (data.validarDeposito != this.validaDepositoEnum.SI) {
      this.messageService.msgWarning(MESSAGES.FORM.REGISTRO_LINEA.WARNING_VALIDA_DEPOSTO, () => { });
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.REGISTRO_LINEA.CONFIRM_DERIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let registroLineaEstado = new RegistroLineaEstado();
        registroLineaEstado.registroLineaId = data.registroLineaId;
        registroLineaEstado.estado = this.estadoRegLineaEnum.DERIVADO;
        registroLineaEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.registroLineaService.updateEstadoRegistroLinea(registroLineaEstado).subscribe(
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

  onDesestimar(data: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.REGISTRO_LINEA.CONFIRM_DESESTIMAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let registroLineaEstado = new RegistroLineaEstado();
        registroLineaEstado.registroLineaId = data.registroLineaId;
        registroLineaEstado.estado = this.estadoRegLineaEnum.DESESTIMADO;
        registroLineaEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.registroLineaService.updateEstadoRegistroLinea(registroLineaEstado).subscribe(
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

  onAutorizar(data: RegistroLinea) {
    this.messageService.msgConfirm(MESSAGES.FORM.REGISTRO_LINEA.CONFIRM_AUTORIZAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let registroLineaEstado = new RegistroLineaEstado();
        registroLineaEstado.registroLineaId = data.registroLineaId;
        registroLineaEstado.estado = this.estadoRegLineaEnum.AUTORIZAR;
        registroLineaEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.registroLineaService.updateEstadoRegistroLinea(registroLineaEstado).subscribe(
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

  onEmitRI(data: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.REGISTRO_LINEA.CONFIRM_EMITIR_RECIBO_INGRESO,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let registroLineaEstado = new RegistroLineaEstado();
        registroLineaEstado.registroLineaId = data.registroLineaId;
        registroLineaEstado.estado = this.estadoRegLineaEnum.EMITIR_RI;
        registroLineaEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.registroLineaService.updateEstadoRegistroLinea(registroLineaEstado).subscribe(
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

  onDelete(data: RegistroLinea) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminado...");
        this.registroLineaService.deleteRegistroLinea(data.registroLineaId).subscribe(
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

export class RegistroLineasDataSource implements DataSource<RegistroLinea> {
  private _dataChange = new BehaviorSubject<any>([]);
  private _loadingChange = new BehaviorSubject<boolean>(false);
  public total = 0;
  public loading = this._loadingChange.asObservable();

  constructor(private RegistroLineaService: RegistroLineaService) { }

  loadRegistroLineas(filter: RegistroLineaFilter) {
    this._loadingChange.next(true);
    this.RegistroLineaService.getRegistroLineasFilter(filter)
      .pipe(
        finalize(() => this._loadingChange.next(false))
      )
      .subscribe(
        (response) => {
          if (response.success) {
            response.data.items.forEach((item: RegistroLinea, index) => {
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
