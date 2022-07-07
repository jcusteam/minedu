
import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { MatDialog } from "@angular/material/dialog";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { CollectionViewer } from "@angular/cdk/collections";
import { finalize, tap } from "rxjs/operators";

import { PapeletaDeposito, PapeletaDepositoEstado, PapeletaDepositoFilter } from "src/app/core/models/PapeletaDeposito";
import { Estado } from "src/app/core/models/estado";
import { PapeletaDepositoService } from "src/app/core/services/papeleta-deposito.service";
import { EstadoService } from "src/app/core/services/estado.service";
import { NewPapeletaDepositoComponent } from "../dialogs/new-papeleta-deposito/new-papeleta-deposito.component";
import { InfoPapeletaDepositoComponent } from "../dialogs/info-papeleta-deposito/info-papeleta-deposito.component";
import { EditPapeletaDepositoComponent } from "../dialogs/edit-papeleta-deposito/edit-papeleta-deposito.component";
import { Banco } from "src/app/core/models/banco";
import { BancoService } from "src/app/core/services/banco.service";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoPapeletaDepositoEnum, MenuEnum, TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-papeleta-deposito",
  templateUrl: "./papeleta-deposito.component.html",
  styleUrls: ["./papeleta-deposito.component.scss"],
})
export class PapeletaDepositoComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  // Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoPapeletaDepositoEnum = EstadoPapeletaDepositoEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "numero",
    "estado",
    "fechaRegistro",
    "banco",
    "cuentaCorriente",
    "monto",
    "actions",
  ];

  filter = new PapeletaDepositoFilter();
  dataSource: PapeletaDepositosDataSource;

  estados: Estado[] = [];
  bancos: Banco[] = [];
  acciones: Accion[] = [];
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  constructor(
    private papeletaDepositoService: PapeletaDepositoService,
    private estadoService: EstadoService,
    private bancoService: BancoService,
    private cuentaCorrienteService: CuentaCorrienteService,
    private dialog: MatDialog,
    private messageService: MessageService,
    private authService: AuthService,
    public fb: FormBuilder,
    private router: Router,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      numero: [null],
      estado: [null],
      bancoId: [null],
      cuentaCorriente: [null],
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;

    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "papeletaDepositoId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;

    this.dataSource = new PapeletaDepositosDataSource(this.papeletaDepositoService);
    this.onlaodAccion();
    this.dataSource.loadPapeletaDepositos(this.filter);

    // Listado de estados
    this.estadoService.getEstadoByTipoDocumento(this.tipoDocEnum.PAPELETA_DEPOSITO)
      .subscribe((response) => {
        if (response.success) {
          this.estados = response.data;
        }
      });

    this.loadMaestras();

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadPapeletaDepositoPage()))
      .subscribe();
  }

  loadMaestras() {
    // Listado de bancos
    this.bancoService.getBancos().subscribe((response) => {
      if (response.success) {
        this.bancos = response.data;
      }
    });
    // Listado de Cuentas corrientes
    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.filter.unidadEjecutoraId)
      .subscribe((response) => {
        if (response.success) {
          response.data.forEach((element) => {
            element.denominacion = element?.numero + " - " + element?.denominacion;
          });
          this.cuentasCorrientes = response.data;
          this.filteredcuentasCorrientes = this.cuentasCorrientes.slice();
        }
      });
  }

  loadPapeletaDepositoPage() {
    this.filter.sortColumn = "papeletaDepositoId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadPapeletaDepositos(this.filter);
  }

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente) {
    this.filter.cuentaCorrienteId = cuentaCorriente.cuentaCorrienteId;
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.PAPELETA_DEPOSITO)
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

  // Buscar
  onSearch(form: PapeletaDepositoFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.filter.numero = form.numero;
    this.filter.bancoId = form.bancoId;
    this.filter.estado = form.estado;

    this.loadPapeletaDepositoPage();
  }

  // Refrescar
  onRefresh() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.loadPapeletaDepositoPage();
  }

  // Limpiar
  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.form.reset();
    this.filter = new PapeletaDepositoFilter();
    this.loadPapeletaDepositoPage();
  }

  // Nuevo
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let papeletaDeposito = new PapeletaDeposito();
    papeletaDeposito.unidadEjecutoraId = +this.settings.unidadEjecutora;
    papeletaDeposito.tipoDocumentoId = this.tipoDocEnum.PAPELETA_DEPOSITO;
    papeletaDeposito.estado = this.estadoPapeletaDepositoEnum.EMITIDO;
    papeletaDeposito.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewPapeletaDepositoComponent, {
      width: "1000px",
      disableClose: true,
      data: papeletaDeposito,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Info
  openDialogInfo(data: PapeletaDeposito): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoPapeletaDepositoComponent, {
      width: "1000px",
      disableClose: true,
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Edit
  openDialogEdit(data: PapeletaDeposito): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditPapeletaDepositoComponent, {
      width: "1000px",
      disableClose: true,
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Procesar
  onProcess(data: PapeletaDeposito) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_PROCESAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let papeletaDepositoEstado = new PapeletaDepositoEstado();
        papeletaDepositoEstado.papeletaDepositoId = data.papeletaDepositoId;
        papeletaDepositoEstado.estado = this.estadoPapeletaDepositoEnum.PROCESADO;
        papeletaDepositoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.papeletaDepositoService.updateEstadoPapeletaDeposito(papeletaDepositoEstado).subscribe(
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

  // Eliminar
  onDelete(data: PapeletaDeposito) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Anulando...");
        this.papeletaDepositoService.deletePapeletaDeposito(data.papeletaDepositoId).subscribe(
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

export class PapeletaDepositosDataSource implements DataSource<PapeletaDeposito> {
  private _dataChange = new BehaviorSubject<any>([]);
  private _loadingChange = new BehaviorSubject<boolean>(false);
  public total = 0;
  public loading = this._loadingChange.asObservable();

  constructor(private PapeletaDepositoService: PapeletaDepositoService) { }

  loadPapeletaDepositos(filter: PapeletaDepositoFilter) {
    this._loadingChange.next(true);
    this.PapeletaDepositoService.getPapeletaDepositosFilter(filter)
      .pipe(
        finalize(() => this._loadingChange.next(false))
      )
      .subscribe(
        (response) => {
          if (response.success) {
            response.data.items.forEach((item: PapeletaDeposito, index) => {
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
