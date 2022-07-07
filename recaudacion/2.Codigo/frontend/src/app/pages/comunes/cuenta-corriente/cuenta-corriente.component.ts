import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";

import {
  CuentaCorriente,
  CuentaCorrienteFilter,
} from "src/app/core/models/cuentacorriente";
import { CuentaCorrienteService } from "src/app/core/services/cuenta-corriente.service";
import { NewCuentaCorrienteComponent } from "./dialogs/new-cuenta-corriente/new-cuenta-corriente.component";
import { EditCuentaCorrienteComponent } from "./dialogs/edit-cuenta-corriente/edit-cuenta-corriente.component";
import { InfoCuentaCorrienteComponent } from "./dialogs/info-cuenta-corriente/info-cuenta-corriente.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";


@Component({
  selector: "app-cuenta-corriente",
  templateUrl: "./cuenta-corriente.component.html",
  styleUrls: ["./cuenta-corriente.component.scss"],
})
export class CuentaCorrienteComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    "index",
    "codigo",
    "numero",
    "denominacion",
    "banco",
    "fuenteFinanciamiento",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new CuentaCorrienteFilter();
  dataSource: CuentaCorrienteDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private cuentaCorrienteService: CuentaCorrienteService,
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
      numero: [null, Validators.compose([
        Validators.pattern("^[a-zA-Z0-9]+$"),
        Validators.maxLength(30)
      ])],
      denominacion: [null, Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(150)
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
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource = new CuentaCorrienteDataSource(this.cuentaCorrienteService);
    this.onlaodAccion();
    this.dataSource.loadCuentaCorrientes(this.filter);

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
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadCuentaCorrientes(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.CUENTA_CORRIENTE)
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

    this.filter = new CuentaCorrienteFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: CuentaCorrienteFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    if (!this.form.valid) {
      return;
    }
    this.filter.numero = form.numero;
    this.filter.denominacion = form.denominacion;
    this.filter.estado = form.estado;
    this.paginator.pageIndex = 0;

    this.loadIntegrantePage();
  }

  // Nuevos
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let cuentaCorriente = new CuentaCorriente();
    cuentaCorriente.estado = true;
    cuentaCorriente.unidadEjecutoraId = +this.settings.unidadEjecutora;
    cuentaCorriente.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewCuentaCorrienteComponent, {
      disableClose: true,
      width: "800px",
      data: cuentaCorriente,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: CuentaCorriente): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditCuentaCorrienteComponent, {
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

    const dialogRef = this.dialog.open(InfoCuentaCorrienteComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(cuentaCorriente: CuentaCorriente) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        cuentaCorriente.estado = true;
        cuentaCorriente.usuarioModificador = this.usuario.numeroDocumento;
        this.cuentaCorrienteService.updateCuentaCorriente(cuentaCorriente)
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

  onInactivar(cuentaCorriente: CuentaCorriente) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        cuentaCorriente.estado = false;
        cuentaCorriente.usuarioModificador = this.usuario.numeroDocumento;
        this.cuentaCorrienteService.updateCuentaCorriente(cuentaCorriente)
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

export class CuentaCorrienteDataSource implements DataSource<CuentaCorriente> {
  private CuentaCorrientesSubject = new BehaviorSubject<CuentaCorriente[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private CuentaCorrienteService: CuentaCorrienteService) { }

  loadCuentaCorrientes(filter: CuentaCorrienteFilter) {
    this.loadingSubject.next(true);
    this.CuentaCorrienteService.getCuentaCorrientesFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: CuentaCorriente, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.CuentaCorrientesSubject.next(response.data.items);
            this.totalItems = response.data.total;
            this.isLoadingResults = false;
          }, 500);

        }
        else {
          this.isLoadingResults = false;
        }
      },
      () => {
        this.isLoadingResults = false;
      }
    );
  }

  connect(collectionViewer: CollectionViewer): Observable<CuentaCorriente[]> {
    return this.CuentaCorrientesSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.CuentaCorrientesSubject.complete();
    this.loadingSubject.complete();
  }
}
