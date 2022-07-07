import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import {
  UnidadEjecutora,
  UnidadEjecutoraFilter,
} from "src/app/core/models/unidadejecutora";
import { UnidadEjecutoraService } from "src/app/core/services/unidad-ejecutora.service";
import { NewUnidadEjecutoraComponent } from "./dialogs/new-unidad-ejecutora/new-unidad-ejecutora.component";
import { EditUnidadEjecutoraComponent } from "./dialogs/edit-unidad-ejecutora/edit-unidad-ejecutora.component";
import { InfoUnidadEjecutoraComponent } from "./dialogs/info-unidad-ejecutora/info-unidad-ejecutora.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-unidad-ejecutora",
  templateUrl: "./unidad-ejecutora.component.html",
  styleUrls: ["./unidad-ejecutora.component.scss"],
})
export class UnidadEjecutoraComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    "index",
    "secuencia",
    "codigo",
    "nombre",
    "numeroRuc",
    "direccion",
    "correo",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new UnidadEjecutoraFilter();
  dataSource: UnidadEjecutoraDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private unidadEjecutoraService: UnidadEjecutoraService,
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
        Validators.pattern('^[0-9]+$'),
        Validators.maxLength(10)
      ])],
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
    this.dataSource = new UnidadEjecutoraDataSource(this.unidadEjecutoraService);
    this.onlaodAccion();
    this.dataSource.loadUnidadEjecutoras(this.filter);
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
    this.dataSource.loadUnidadEjecutoras(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.UNIDA_EJECUTORA)
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

    this.filter = new UnidadEjecutoraFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form?: UnidadEjecutoraFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    if (!this.form.valid)
      return;

    this.filter.codigo = form.codigo;
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

    let unidadEjecutora = new UnidadEjecutora();
    unidadEjecutora.estado = true;
    unidadEjecutora.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewUnidadEjecutoraComponent, {
      disableClose: true,
      width: "800px",
      data: unidadEjecutora,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: UnidadEjecutora): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditUnidadEjecutoraComponent, {
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

    const dialogRef = this.dialog.open(InfoUnidadEjecutoraComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(unidadEjecutora: UnidadEjecutora) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        unidadEjecutora.estado = true;
        unidadEjecutora.usuarioModificador = this.usuario.numeroDocumento;
        this.unidadEjecutoraService.updateUnidadEjecutora(unidadEjecutora)
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

  onInactivar(unidadEjecutora: UnidadEjecutora) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        unidadEjecutora.estado = false;
        unidadEjecutora.usuarioModificador = this.usuario.numeroDocumento;
        this.unidadEjecutoraService.updateUnidadEjecutora(unidadEjecutora)
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

export class UnidadEjecutoraDataSource implements DataSource<UnidadEjecutora> {
  private UnidadEjecutorasSubject = new BehaviorSubject<UnidadEjecutora[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private UnidadEjecutoraService: UnidadEjecutoraService) { }

  loadUnidadEjecutoras(filter: UnidadEjecutoraFilter) {
    this.loadingSubject.next(true);
    this.UnidadEjecutoraService.getUnidadEjecutorasFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: UnidadEjecutora, index) => {
              item.index =
                (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.UnidadEjecutorasSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<UnidadEjecutora[]> {
    return this.UnidadEjecutorasSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.UnidadEjecutorasSubject.complete();
    this.loadingSubject.complete();
  }
}
