import { TipoDocumentoEstado } from './../../../core/models/tipodocumento';
import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";

import {
  TipoDocumento,
  TipoDocumentoFilter,
} from "src/app/core/models/tipodocumento";
import { TipoDocumentoService } from "src/app/core/services/tipo-documento.service";
import { NewTipoDocumentoComponent } from "./dialogs/new-tipo-documento/new-tipo-documento.component";
import { EditTipoDocumentoComponent } from "./dialogs/edit-tipo-documento/edit-tipo-documento.component";
import { InfoTipoDocumentoComponent } from "./dialogs/info-tipo-documento/info-tipo-documento.component";
import { ListTipoDocumentoEstadoComponent } from "./dialogs/list-tipo-documento-estado/list-tipo-documento-estado.component";
import { EditTipoDocumentoParametroComponent } from "./dialogs/edit-tipo-documento-parametro/edit-tipo-documento-parametro.component";
import { MessageService } from "src/app/core/services/message.service";
import { Settings } from "src/app/app.settings.model";
import { AuthService } from "src/app/core/services/auth.service";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { TransversalService } from "src/app/core/services/transversal.service";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { Parametro } from "src/app/core/models/parametro";


@Component({
  selector: "app-tipo-documento",
  templateUrl: "./tipo-documento.component.html",
  styleUrls: ["./tipo-documento.component.scss"],
})
export class TipoDocumentoComponent implements OnInit {
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

  filter = new TipoDocumentoFilter();
  dataSource: TipoDocumentoDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private tipoDocumentoService: TipoDocumentoService,
    private transversalService: TransversalService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public fb: FormBuilder,
    public appSettings: AppSettings
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
    this.filter.sortColumn = "tipoDocumentoId";
    this.filter.sortOrder = "asc";
    this.dataSource = new TipoDocumentoDataSource(this.tipoDocumentoService);
    this.onlaodAccion();
    this.dataSource.loadTipoDocumentos(this.filter);
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
    this.filter.sortColumn = "tipoDocumentoId";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadTipoDocumentos(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.TIPO_DOCUMENTO)
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
    this.loadIntegrantePage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter = new TipoDocumentoFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: TipoDocumentoFilter) {
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
    let tipoDocumento = new TipoDocumento();
    tipoDocumento.estado = true;
    tipoDocumento.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewTipoDocumentoComponent, {
      disableClose: true,
      width: "800px",
      data: tipoDocumento,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: TipoDocumento): void {
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditTipoDocumentoComponent, {
      disableClose: true,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(data): void {
    const dialogRef = this.dialog.open(InfoTipoDocumentoComponent, {
      disableClose: true,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEstado(data: TipoDocumento) {
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(ListTipoDocumentoEstadoComponent, {
      disableClose: true,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogParametro(data: TipoDocumento) {
    let parametro = new Parametro();
    parametro.unidadEjecutoraId = +this.settings.unidadEjecutora;
    parametro.tipoDocumentoId = data.tipoDocumentoId;
    parametro.estado = true;
    parametro.usuarioModificador = this.usuario.numeroDocumento;
    parametro.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditTipoDocumentoParametroComponent, {
      disableClose: true,
      width: "600px",
      data: parametro,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(tipoDocumento: TipoDocumento) {
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let tipoDocEstado = new TipoDocumentoEstado();
        tipoDocEstado.tipoDocumentoId = tipoDocumento.tipoDocumentoId;
        tipoDocEstado.estado = true;
        tipoDocEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.tipoDocumentoService.updateEstadoTipoDocumento(tipoDocEstado).subscribe(
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

  onInactivar(tipoDocumento: TipoDocumento) {
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let tipoDocEstado = new TipoDocumentoEstado();
        tipoDocEstado.tipoDocumentoId = tipoDocumento.tipoDocumentoId;
        tipoDocEstado.estado = false;
        tipoDocEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.tipoDocumentoService.updateEstadoTipoDocumento(tipoDocEstado).subscribe(
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

export class TipoDocumentoDataSource implements DataSource<TipoDocumento> {
  private tipoDocumentosSubject = new BehaviorSubject<TipoDocumento[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private tipoDocumentoService: TipoDocumentoService) { }

  loadTipoDocumentos(filter: TipoDocumentoFilter) {
    this.loadingSubject.next(true);
    this.tipoDocumentoService
      .getTipoDocumentosFilter(filter)
      .subscribe((response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: TipoDocumento, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.tipoDocumentosSubject.next(response.data.items);
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
        });
  }

  connect(collectionViewer: CollectionViewer): Observable<TipoDocumento[]> {
    return this.tipoDocumentosSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.tipoDocumentosSubject.complete();
    this.loadingSubject.complete();
  }

  get data(): any {
    return this.tipoDocumentosSubject.value || [];
  }
}
