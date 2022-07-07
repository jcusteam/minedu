import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import {
  CuentaContable,
  CuentaContableFilter,
} from "src/app/core/models/cuentacontable";
import { CuentaContableService } from "src/app/core/services/cuenta-contable.service";
import { NewCuentaContableComponent } from "./dialogs/new-cuenta-contable/new-cuenta-contable.component";
import { EditCuentaContableComponent } from "./dialogs/edit-cuenta-contable/edit-cuenta-contable.component";
import { InfoCuentaContableComponent } from "./dialogs/info-cuenta-contable/info-cuenta-contable.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-cuenta-contable",
  templateUrl: "./cuenta-contable.component.html",
  styleUrls: ["./cuenta-contable.component.scss"],
})
export class CuentaContableComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    "index",
    "codigo",
    "descripcion",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new CuentaContableFilter();
  dataSource: CuentaContableDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private cuentaContableService: CuentaContableService,
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
      codigo: [null, Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(13)
      ])],
      descripcion: [null, Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(200)
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
    this.dataSource = new CuentaContableDataSource(this.cuentaContableService);
    this.onlaodAccion();
    this.dataSource.loadCuentaContables(this.filter);
    
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

  loadIntegrantePage() {
    this.filter.sortColumn = "codigo";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadCuentaContables(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.CUENTA_CONTABLE)
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

    this.loadIntegrantePage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter = new CuentaContableFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: CuentaContableFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    if (!this.form.valid) {
      return;
    }
    this.filter.codigo = form.codigo;
    this.filter.descripcion = form.descripcion;
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
    let cuentaContable = new CuentaContable();

    cuentaContable.estado = true;
    cuentaContable.usuarioCreador = this.usuario.numeroDocumento;

    const dialogRef = this.dialog.open(NewCuentaContableComponent, {
      disableClose: true,
      width: "800px",
      data: cuentaContable,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: CuentaContable): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditCuentaContableComponent, {
      disableClose: true,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(data): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoCuentaContableComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(cuentaContable: CuentaContable) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        cuentaContable.estado = true;
        cuentaContable.usuarioModificador = this.usuario.numeroDocumento;
        this.cuentaContableService.updateCuentaContable(cuentaContable)
          .subscribe(
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

  onInactivar(cuentaContable: CuentaContable) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        cuentaContable.estado = false;
        cuentaContable.usuarioModificador = this.usuario.numeroDocumento;
        this.cuentaContableService.updateCuentaContable(cuentaContable)
          .subscribe(
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

  // Respone
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

export class CuentaContableDataSource implements DataSource<CuentaContable> {
  private CuentaContablesSubject = new BehaviorSubject<CuentaContable[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private CuentaContableService: CuentaContableService) { }

  loadCuentaContables(filter: CuentaContableFilter) {
    this.loadingSubject.next(true);
    this.CuentaContableService.getCuentaContablesFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: CuentaContable, index) => {
              item.index =
                (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.CuentaContablesSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<CuentaContable[]> {
    return this.CuentaContablesSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.CuentaContablesSubject.complete();
    this.loadingSubject.complete();
  }
}
