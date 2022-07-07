import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { Tarifario, TarifarioFilter } from "src/app/core/models/tarifario";
import { TarifarioService } from "src/app/core/services/tarifario.service";
import { NewTarifarioComponent } from "./dialogs/new-tarifario/new-tarifario.component";
import { EditTarifarioComponent } from "./dialogs/edit-tarifario/edit-tarifario.component";
import { InfoTarifarioComponent } from "./dialogs/info-tarifario/info-tarifario.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { AppSettings } from "src/app/app.settings";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { AccionEnum, EstadoEnum, MenuEnum, PrecioVariableEnum } from "src/app/core/enums/recaudacion.enum";

@Component({
  selector: "app-tarifario",
  templateUrl: "./tarifario.component.html",
  styleUrls: ["./tarifario.component.scss"],
})
export class TarifarioComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    "index",
    "codigo",
    "nombre",
    "porcentajeUit",
    "precio",
    "clasificadorIngreso",
    "grupoRecaudacion",
    "precioVariable",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;
  precioVariableEnum = PrecioVariableEnum;

  filter = new TarifarioFilter();
  dataSource: TarifarioDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private tarifarioService: TarifarioService,
    private transversalService: TransversalService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private authService: AuthService,
    public appSettings: AppSettings,
    private router: Router,
    public fb: FormBuilder,
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      codigo: [null, Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(6)
      ])],
      nombre: [null, Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(300)
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
    this.dataSource = new TarifarioDataSource(this.tarifarioService);
    this.onlaodAccion();
    this.dataSource.loadTarifarios(this.filter);

    this.transversalService.getEstados().subscribe(
      response => this.estados = response.data
    );

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadTarifarioPage()))
      .subscribe();
  }

  loadTarifarioPage() {
    this.filter.sortColumn = "codigo";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadTarifarios(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.TARIFARIO)
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

    this.loadTarifarioPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      return;
    }

    this.filter = new TarifarioFilter();
    this.form.reset();
    this.loadTarifarioPage();
  }

  onSearch(form: TarifarioFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter.codigo = form.codigo;
    this.filter.nombre = form.nombre;
    this.filter.estado = form.estado;

    this.loadTarifarioPage();
  }

  onPrecioVariable(tipo: boolean) {
    var variable = "Si";
    if (!tipo) {
      variable = "No";
    }
    return variable;
  }

  // Nuevo
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let tarifario = new Tarifario();
    tarifario.estado = true;
    tarifario.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewTarifarioComponent, {
      disableClose: true,
      width: "800px",
      data: tarifario
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: Tarifario): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditTarifarioComponent, {
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

    const dialogRef = this.dialog.open(InfoTarifarioComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(tarifario: Tarifario) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        tarifario.estado = true;
        tarifario.usuarioModificador = this.usuario.numeroDocumento;
        this.tarifarioService.updateTarifario(tarifario).subscribe(
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

  onInactivar(tarifario: Tarifario) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        tarifario.estado = false;
        tarifario.usuarioModificador = this.usuario.numeroDocumento;
        this.tarifarioService.updateTarifario(tarifario).subscribe(
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

export class TarifarioDataSource implements DataSource<Tarifario> {
  private tarifariosSubject = new BehaviorSubject<Tarifario[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private TarifarioService: TarifarioService) { }

  loadTarifarios(filter: TarifarioFilter) {
    this.loadingSubject.next(true);
    this.TarifarioService.getTarifariosFilter(filter).subscribe(
      (response) => {
        this.isLoadingResults = false;
        if (response.success) {
          response.data.items.forEach((item: Tarifario, index) => {
            item.index =
              (filter.pageNumber - 1) * filter.pageSize + (index + 1);
          });
          this.tarifariosSubject.next(response.data.items);
          this.totalItems = response.data.total;
        }
      },
      () => {
        this.isLoadingResults = false;
      }
    );
  }

  connect(collectionViewer: CollectionViewer): Observable<Tarifario[]> {
    return this.tarifariosSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.tarifariosSubject.complete();
    this.loadingSubject.complete();
  }
}
