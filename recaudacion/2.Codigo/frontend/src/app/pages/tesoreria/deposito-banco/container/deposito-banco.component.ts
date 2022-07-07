import { DepositoBancoEstado } from './../../../../core/models/depositobanco';
import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { Observable, merge, BehaviorSubject } from "rxjs";
import { tap, finalize } from "rxjs/operators";
import { FormBuilder, FormGroup } from "@angular/forms";
import { DataSource } from "@angular/cdk/table";
import { CollectionViewer } from "@angular/cdk/collections";

import { NewDepositoBancoComponent } from "../dialogs/new-deposito-banco/new-deposito-banco.component";
import { EditDepositoBancoComponent } from "../dialogs/edit-deposito-banco/edit-deposito-banco.component";

import { DepositoBancoService } from "src/app/core/services/deposito-banco.service";
import {
  DepositoBanco,
  DepositoBancoFilter,
} from "src/app/core/models/depositobanco";
import { Estado } from "src/app/core/models/estado";
import { EstadoService } from "src/app/core/services/estado.service";
import { Banco } from "src/app/core/models/banco";
import { BancoService } from "src/app/core/services/banco.service";
import { CuentaCorriente } from "src/app/core/models/cuentacorriente";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { AppSettings } from "src/app/app.settings";
import { Router } from "@angular/router";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoDepositoBancoEnum, MenuEnum, TipoDocEnum, TipoDocIdentidadEnum } from "src/app/core/enums/recaudacion.enum";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-deposito-banco",
  templateUrl: "./deposito-banco.component.html",
  styleUrls: ["./deposito-banco.component.scss"],
})
export class DepositoBancoComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  estadoDepositoBancoEnum = EstadoDepositoBancoEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "fechaDeposito",
    "mes",
    "anio",
    "estado",
    "banco",
    "cuentaCorriente",
    "cantidad",
    "importe",
    "actions",
  ];

  filter = new DepositoBancoFilter();
  // DataSource
  dataSource: DepositoBancosDataSource;

  estados: Estado[] = [];
  bancos: Banco[] = [];
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredcuentasCorrientes;
  acciones: Accion[] = [];

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  constructor(
    public dialog: MatDialog,
    private depositoBancoService: DepositoBancoService,
    private estadoService: EstadoService,
    private bancoService: BancoService,
    private cuentaCorrienteService: CuentaCorrienteService,
    private messageService: MessageService,
    private router: Router,
    public fb: FormBuilder,
    private authService: AuthService,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;

    this.form = this.fb.group({
      cuentaCorriente: [null],
      bancoId: [null],
      estado: [null]
    });

  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;

    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "depositoBancoId";
    this.filter.sortOrder = "desc";

    this.dataSource = new DepositoBancosDataSource(this.depositoBancoService);
    this.onlaodAccion();
    this.dataSource.loadDepositoBancos(this.filter);

    this.loadMaestras();
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadDepositoBancoPage()))
      .subscribe();
  }

  // Cuenta Corriente
  seletedChangeCuentaCorriente(cuentaCorriente) {
    this.filter.cuentaCorrienteId = cuentaCorriente.cuentaCorrienteId;
  }

  loadMaestras() {
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

    this.estadoService.getEstadoByTipoDocumento(this.tipoDocEnum.DEPOSITO_BANCO).subscribe((response) => {
      if (response.success) {
        this.estados = response.data;
      }
    });

    this.bancoService.getBancos().subscribe((response) => {
      if (response.data) {
        this.bancos = response.data;
      }
    });

  }

  loadDepositoBancoPage() {
    this.filter.sortColumn = "depositoBancoId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadDepositoBancos(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.DEPOSITO_BANCO)
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

    this.loadDepositoBancoPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.form.reset();
    this.filter = new DepositoBancoFilter();
    this.loadDepositoBancoPage();
  }

  onSearch(form: DepositoBancoFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter.bancoId = form.bancoId;
    this.filter.estado = form.estado;
    this.loadDepositoBancoPage();
  }

  // Nuevo
  openDialogNew(): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let depositoBanco = new DepositoBanco();
    depositoBanco.unidadEjecutoraId = +this.settings.unidadEjecutora;
    depositoBanco.tipoDocumentoId = this.tipoDocEnum.DEPOSITO_BANCO;
    depositoBanco.estado = this.estadoDepositoBancoEnum.EMITIDO;
    depositoBanco.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewDepositoBancoComponent, {
      disableClose: true,
      width: "1200px",
      data: depositoBanco,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Editar
  openDialogEdit(data: DepositoBanco): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditDepositoBancoComponent, {
      disableClose: true,
      width: "1200px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Eliminar
  onDelete(depositobanco: DepositoBanco): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminado...");
        this.depositoBancoService.deleteDepositoBanco(depositobanco.depositoBancoId).subscribe(
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

  // Procesar
  onProcess(data: DepositoBanco): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_PROCESAR,
      () => {
        this.messageService.msgLoad("Procesando...");
        let depositoBancoEstado = new DepositoBancoEstado();
        depositoBancoEstado.depositoBancoId = data.depositoBancoId;
        depositoBancoEstado.estado = this.estadoDepositoBancoEnum.PROCESADO;
        depositoBancoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.depositoBancoService.updateEstadoDepositoBanco(depositoBancoEstado).subscribe(
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

export class DepositoBancosDataSource implements DataSource<DepositoBanco> {
  private _dataChange = new BehaviorSubject<any>([]);
  private _loadingChange = new BehaviorSubject<boolean>(false);
  public total = 0;
  public loading = this._loadingChange.asObservable();

  constructor(private depositoBancoService: DepositoBancoService) { }

  loadDepositoBancos(filter: DepositoBancoFilter) {
    this._loadingChange.next(true);
    this.depositoBancoService.getDepositoBancosFilter(filter)
      .pipe(
        finalize(() => this._loadingChange.next(false))
      )
      .subscribe(
        (response) => {
          if (response.success) {
            response.data.items.forEach((item: DepositoBanco, index) => {
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
