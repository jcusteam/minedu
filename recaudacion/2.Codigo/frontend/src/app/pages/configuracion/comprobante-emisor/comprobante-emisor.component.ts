import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";

import { ComprobanteEmisorService } from "src/app/core/services/comprobante-emisor.service";
import { NewComprobanteEmisorComponent } from "./dialogs/new-comprobante-emisor/new-comprobante-emisor.component";
import { EditComprobanteEmisorComponent } from "./dialogs/edit-comprobante-emisor/edit-comprobante-emisor.component";
import { InfoComprobanteEmisorComponent } from "./dialogs/info-comprobante-emisor/info-comprobante-emisor.component";
import {
  ComprobanteEmisor,
  ComprobanteEmisorEstado,
  ComprobanteEmisorFilter,
} from "src/app/core/models/comprobante-emisor";
import { UnidadEjecutora } from "src/app/core/models/unidadejecutora";
import { UnidadEjecutoraService } from "src/app/core/services/unidad-ejecutora.service";
import { AuthService } from "src/app/core/services/auth.service";
import { MessageService } from "src/app/core/services/message.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Combobox } from "src/app/core/interfaces/combobox";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";
import { TransversalService } from "src/app/core/services/transversal.service";

@Component({
  selector: "app-comprobante-emisor",
  templateUrl: "./comprobante-emisor.component.html",
  styleUrls: ["./comprobante-emisor.component.scss"],
})
export class ComprobanteEmisorComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    "index",
    "firmante",
    "numeroRuc",
    "razonSocial",
    "direccion",
    "departamento",
    "provincia",
    "distrito",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;
  
  filter = new ComprobanteEmisorFilter();
  dataSource: ComprobanteEmisorDataSource;
  
  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  unidadesEjecutoras: UnidadEjecutora[] = [];
  acciones: Accion[] = [];

  constructor(
    private ComprobanteEmisorService: ComprobanteEmisorService,
    private dialog: MatDialog,
    private unidadEjecutoraService: UnidadEjecutoraService,
    private transversalService: TransversalService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public fb: FormBuilder,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      unidadEjecutoraId: [null],
      estado: [null]
    });
  }
  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "comprobanteEmisorId";
    this.filter.sortOrder = "asc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.form.patchValue({ unidadEjecutoraId: this.filter?.unidadEjecutoraId });
    this.dataSource = new ComprobanteEmisorDataSource(this.ComprobanteEmisorService);
    this.onlaodAccion();
    this.dataSource.loadComprobanteEmisors(this.filter);
    this.loadMaster();
    
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadIntegrantePage()))
      .subscribe();
  }

  loadMaster(){

    this.unidadEjecutoraService.getUnidadEjecutoras().subscribe((response) => {
      if (response.success) {
        this.unidadesEjecutoras = response.data.filter(x => x.unidadEjecutoraId == this.filter?.unidadEjecutoraId);
      }
    });

    this.transversalService.getEstados().subscribe(
      response => this.estados = response.data
    );
  }

  loadIntegrantePage() {
    this.filter.sortColumn = "comprobanteEmisorId";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadComprobanteEmisors(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.COMPROBANTE_EMISOR)
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

    this.filter = new ComprobanteEmisorFilter();
    this.form.get("estado").reset();
    this.loadIntegrantePage();
  }

  onSearch(form: ComprobanteEmisorFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.filter.unidadEjecutoraId = form.unidadEjecutoraId;
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
    let comprobanteEmisor = new ComprobanteEmisor();
    comprobanteEmisor.estado = true;
    comprobanteEmisor.unidadEjecutoraId = +this.settings.unidadEjecutora;
    comprobanteEmisor.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewComprobanteEmisorComponent, {
      disableClose: true,
      width: "1200px",
      data: comprobanteEmisor,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: ComprobanteEmisor): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditComprobanteEmisorComponent, {
      disableClose: true,
      width: "1200px",
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

    const dialogRef = this.dialog.open(InfoComprobanteEmisorComponent, {
      disableClose: true,
      width: "1200px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(data: ComprobanteEmisor) {
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let comprobanteEmisorEstado = new ComprobanteEmisorEstado();
        comprobanteEmisorEstado.comprobanteEmisorId = data.comprobanteEmisorId;
        comprobanteEmisorEstado.estado = true;
        comprobanteEmisorEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.ComprobanteEmisorService.updateEstadoComprobanteEmisor(comprobanteEmisorEstado).subscribe(
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

  onInactivar(data: ComprobanteEmisor) {
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let comprobanteEmisorEstado = new ComprobanteEmisorEstado();
        comprobanteEmisorEstado.comprobanteEmisorId = data.comprobanteEmisorId;
        comprobanteEmisorEstado.estado = false;
        comprobanteEmisorEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.ComprobanteEmisorService.updateEstadoComprobanteEmisor(comprobanteEmisorEstado).subscribe(
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

export class ComprobanteEmisorDataSource
  implements DataSource<ComprobanteEmisor> {
  private ComprobanteEmisorsSubject = new BehaviorSubject<ComprobanteEmisor[]>(
    []
  );
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private ComprobanteEmisorService: ComprobanteEmisorService) { }

  loadComprobanteEmisors(filter: ComprobanteEmisorFilter) {
    this.loadingSubject.next(true);
    this.ComprobanteEmisorService.getComprobanteEmisorsFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: ComprobanteEmisor, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.ComprobanteEmisorsSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<ComprobanteEmisor[]> {
    return this.ComprobanteEmisorsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.ComprobanteEmisorsSubject.complete();
    this.loadingSubject.complete();
  }
}
