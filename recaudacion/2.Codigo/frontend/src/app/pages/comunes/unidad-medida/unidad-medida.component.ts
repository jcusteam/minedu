import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import {
  UnidadMedida,
  UnidadMedidaFilter,
} from "src/app/core/models/unidadmedida";
import { UnidadMedidaService } from "src/app/core/services/unidad-medida.service";
import { NewUnidadMedidaComponent } from "./dialogs/new-unidad-medida/new-unidad-medida.component";
import { EditUnidadMedidaComponent } from "./dialogs/edit-unidad-medida/edit-unidad-medida.component";
import { InfoUnidadMedidaComponent } from "./dialogs/info-unidad-medida/info-unidad-medida.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-unidad-medida",
  templateUrl: "./unidad-medida.component.html",
  styleUrls: ["./unidad-medida.component.scss"],
})
export class UnidadMedidaComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    "index",
    "nombre",
    "abreviatura",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new UnidadMedidaFilter();
  dataSource: UnidadMedidaDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private unidadMedidaService: UnidadMedidaService,
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
    this.filter.pageSize = 5;
    this.filter.sortColumn = "nombre";
    this.filter.sortOrder = "asc";
    this.dataSource = new UnidadMedidaDataSource(this.unidadMedidaService);
    this.onlaodAccion();
    this.dataSource.loadUnidadMedidas(this.filter);
    
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
    this.filter.sortColumn = "nombre";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadUnidadMedidas(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.UNIDAD_MEDIDA)
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

    this.filter = new UnidadMedidaFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: UnidadMedidaFilter) {
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
    let unidadMedida = new UnidadMedida();
    unidadMedida.estado = true;
    unidadMedida.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewUnidadMedidaComponent, {
      disableClose: true,
      width: "800px",
      data: unidadMedida,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: UnidadMedida): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditUnidadMedidaComponent, {
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

    const dialogRef = this.dialog.open(InfoUnidadMedidaComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(unidadMedida: UnidadMedida) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        unidadMedida.estado = true;
        unidadMedida.usuarioModificador = this.usuario.numeroDocumento;
        this.unidadMedidaService.updateUnidadMedida(unidadMedida).subscribe(
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

  onInactivar(unidadMedida: UnidadMedida) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        unidadMedida.estado = false;
        unidadMedida.usuarioModificador = this.usuario.numeroDocumento;
        this.unidadMedidaService.updateUnidadMedida(unidadMedida).subscribe(
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

export class UnidadMedidaDataSource implements DataSource<UnidadMedida> {
  private unidadMedidasSubject = new BehaviorSubject<UnidadMedida[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private UnidadMedidaService: UnidadMedidaService) { }

  loadUnidadMedidas(filter: UnidadMedidaFilter) {
    this.loadingSubject.next(true);
    this.UnidadMedidaService.getUnidadMedidasFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: UnidadMedida, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.unidadMedidasSubject.next(response.data.items);
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

  connect(_collectionViewer: CollectionViewer): Observable<UnidadMedida[]> {
    return this.unidadMedidasSubject.asObservable();
  }

  disconnect(_collectionViewer: CollectionViewer): void {
    this.unidadMedidasSubject.complete();
    this.loadingSubject.complete();
  }
}
