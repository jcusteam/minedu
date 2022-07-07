import { Component, OnInit, ViewChild } from "@angular/core";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { merge, BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { DataSource } from "@angular/cdk/table";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { ClienteService } from "src/app/core/services/cliente.service";
import { Cliente, ClienteFilter } from "src/app/core/models/cliente";
import { NewClienteComponent } from "./dialogs/new-cliente/new-cliente.component";
import { EditClienteComponent } from "./dialogs/edit-cliente/edit-cliente.component";
import { InfoClienteComponent } from "./dialogs/info-cliente/info-cliente.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-cliente",
  templateUrl: "./cliente.component.html",
  styleUrls: ["./cliente.component.scss"],
})
export class ClienteComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    "index",
    "tipoDocumento",
    "numeroDocumento",
    "nombre",
    "direccion",
    "correo",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new ClienteFilter();
  dataSource: ClienteDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    public dialog: MatDialog,
    private clienteService: ClienteService,
    private transversalService: TransversalService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public appSettings: AppSettings,
    public fb: FormBuilder,
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      numeroDocumento: [null, Validators.compose([
        Validators.pattern("^[a-zA-Z0-9]+$"),
        Validators.maxLength(12)
      ])],
      nombre: [null, Validators.compose([
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
    this.filter.sortColumn = "nombre";
    this.filter.sortOrder = "asc";
    this.dataSource = new ClienteDataSource(this.clienteService);
    this.onlaodAccion();
    this.dataSource.loadClientes(this.filter);

    this.transversalService.getEstados().subscribe(
      response => this.estados = response.data
    );
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadClientePage()))
      .subscribe();
  }

  loadClientePage() {
    this.filter.sortColumn = "nombre";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadClientes(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.CLIENTE)
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

    this.loadClientePage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter = new ClienteFilter();
    this.form.reset();
    this.loadClientePage();
  }

  onSearch(form: ClienteFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid) {
      return;
    }

    this.filter.numeroDocumento = form.numeroDocumento;
    this.filter.nombre = form.nombre;
    this.filter.estado = form.estado;
    this.paginator.pageIndex = 0;

    this.loadClientePage();
  }

  // Nuevo
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let cliente = new Cliente();
    cliente.estado = true;
    cliente.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewClienteComponent, {
      disableClose: true,
      width: "800px",
      data: cliente,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: Cliente): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    data.usuarioModificador = this.usuario.numeroDocumento;

    const dialogRef = this.dialog.open(EditClienteComponent, {
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

    const dialogRef = this.dialog.open(InfoClienteComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(cliente: Cliente) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        cliente.estado = true;
        cliente.usuarioModificador = this.usuario.numeroDocumento;
        this.clienteService.updateCliente(cliente).subscribe(
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

  onInactivar(cliente: Cliente) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        cliente.estado = false;
        cliente.usuarioModificador = this.usuario.numeroDocumento;
        this.clienteService.updateCliente(cliente).subscribe(
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

export class ClienteDataSource implements DataSource<Cliente> {
  private ClientesSubject = new BehaviorSubject<Cliente[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private ClienteService: ClienteService) { }

  loadClientes(filter: ClienteFilter) {
    this.loadingSubject.next(true);
    this.ClienteService.getClientesFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: Cliente, index) => {
              item.index =
                (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.ClientesSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<Cliente[]> {
    return this.ClientesSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.ClientesSubject.complete();
    this.loadingSubject.complete();
  }
}
