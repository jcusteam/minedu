import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { delay, finalize, tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";

import { Banco, BancoFilter } from "src/app/core/models/banco";
import { BancoService } from "src/app/core/services/banco.service";
import { NewBancoComponent } from "./dialogs/new-banco/new-banco.component";
import { EditBancoComponent } from "./dialogs/edit-banco/edit-banco.component";
import { InfoBancoComponent } from "./dialogs/info-banco/info-banco.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { AppSettings } from "src/app/app.settings";
import { Router } from "@angular/router";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-banco",
  templateUrl: "./banco.component.html",
  styleUrls: ["./banco.component.scss"],
})
export class BancoComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    "index",
    "codigo",
    "nombre",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new BancoFilter();
  dataSource: BancoDataSource;
  
  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private bancoService: BancoService,
    private transversalService: TransversalService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public appSettings: AppSettings,
    public fb: FormBuilder,
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      nombre: [null, Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(50)
      ])],
      estado: [null]
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "codigo";
    this.filter.sortOrder = "asc";
    this.onlaodAccion();

    this.dataSource = new BancoDataSource(this.bancoService);
    this.dataSource.loadBancos(this.filter);
    this.transversalService.getEstados().subscribe(
      response => this.estados = response.data
    );
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadIntegrantePage()))
      .subscribe();
  }

  // listar bancos
  loadIntegrantePage() {
    this.filter.sortColumn = "codigo";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadBancos(this.filter);
  }

  // listar acciones
  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.BANCO)
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

  // Refrescar la grilla
  onRefresh() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.loadIntegrantePage();
  }

  // Limpiar formulario
  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.filter = new BancoFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  // Buscar
  onSearch(form: BancoFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid)
      return;

    this.filter.nombre = form.nombre;
    this.filter.estado = form.estado;
    this.paginator.pageIndex = 0;
    this.loadIntegrantePage();
  }

  // Nuevo
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let banco = new Banco();
    banco.estado = true;
    banco.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewBancoComponent, {
      disableClose: true,
      width: "800px",
      data: banco,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Editar
  openDialogEdit(data: Banco): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditBancoComponent, {
      disableClose: true,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Info
  openDialogInfo(data): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoBancoComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  // Activar
  onActivar(banco: Banco) {
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        banco.estado = true;
        banco.usuarioModificador = this.usuario.numeroDocumento;
        this.bancoService.updateBanco(banco).subscribe(
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

  // Inactivar
  onInactivar(banco: Banco) {
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        banco.estado = false;
        banco.usuarioModificador = this.usuario.numeroDocumento;
        this.bancoService.updateBanco(banco).subscribe(
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

export class BancoDataSource implements DataSource<any> {
  private _dataChange = new BehaviorSubject<any>([]);
  private _loadingChange = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading = this._loadingChange.asObservable();

  constructor(private BancoService: BancoService) { }

  loadBancos(filter: BancoFilter) {
    this._loadingChange.next(true);
    this.BancoService.getBancosFilter(filter)
      .pipe(
        delay(500),
        finalize(() => this._loadingChange.next(false))
      )
      .subscribe(
        (response) => {
          if (response.success) {
            response.data.items.forEach((item: Banco, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this._dataChange.next(response.data.items);
            this.totalItems = response.data.total;
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
