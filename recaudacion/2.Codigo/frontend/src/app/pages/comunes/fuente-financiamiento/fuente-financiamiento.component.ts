import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";

import {
  FuenteFinanciamiento,
  FuenteFinanciamientoFilter,
} from "src/app/core/models/fuentefinanciamiento";
import { FuenteFinanciamientoService } from "src/app/core/services/fuente-financiamiento.service";
import { NewFuenteFinanciamientoComponent } from "./dialogs/new-fuente-financiamiento/new-fuente-financiamiento.component";
import { EditFuenteFinanciamientoComponent } from "./dialogs/edit-fuente-financiamiento/edit-fuente-financiamiento.component";
import { InfoFuenteFinanciamientoComponent } from "./dialogs/info-fuente-financiamiento/info-fuente-financiamiento.component";
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
  selector: "app-fuente-financiamiento",
  templateUrl: "./fuente-financiamiento.component.html",
  styleUrls: ["./fuente-financiamiento.component.scss"],
})
export class FuenteFinanciamientoComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    "index",
    "codigo",
    "descripcion",
    "rubroCodigo",
    "rubroDescripcion",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;
  
  filter = new FuenteFinanciamientoFilter();
  dataSource: FuenteFinanciamientoDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private fuenteFinanciamientoService: FuenteFinanciamientoService,
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
        Validators.maxLength(2)
      ])],
      descripcion: [null, Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(100)
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
    this.dataSource = new FuenteFinanciamientoDataSource(this.fuenteFinanciamientoService);
    this.onlaodAccion();
    this.dataSource.loadFuenteFinanciamientos(this.filter);

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
    this.dataSource.loadFuenteFinanciamientos(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.FUENTE_FINANCIAMIENTO)
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

    this.filter = new FuenteFinanciamientoFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: FuenteFinanciamientoFilter) {
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
  // Nuevos
  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let fuenteFinanciamiento = new FuenteFinanciamiento();
    fuenteFinanciamiento.estado = true;
    fuenteFinanciamiento.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewFuenteFinanciamientoComponent, {
      disableClose: true,
      width: "800px",
      data: fuenteFinanciamiento,
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: FuenteFinanciamiento): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditFuenteFinanciamientoComponent, {
      disableClose: true,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(data: FuenteFinanciamiento): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoFuenteFinanciamientoComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(fuenteFinanciamiento: FuenteFinanciamiento) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        fuenteFinanciamiento.estado = true;
        fuenteFinanciamiento.usuarioModificador = this.usuario.numeroDocumento;
        this.fuenteFinanciamientoService.updateFuenteFinanciamiento(fuenteFinanciamiento)
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

  onInactivar(fuenteFinanciamiento: FuenteFinanciamiento) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(
      MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        fuenteFinanciamiento.estado = false;
        fuenteFinanciamiento.usuarioModificador = "Admin";
        this.fuenteFinanciamientoService.updateFuenteFinanciamiento(fuenteFinanciamiento)
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

export class FuenteFinanciamientoDataSource
  implements DataSource<FuenteFinanciamiento> {
  private FuenteFinanciamientosSubject = new BehaviorSubject<
    FuenteFinanciamiento[]
  >([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(
    private FuenteFinanciamientoService: FuenteFinanciamientoService
  ) { }

  loadFuenteFinanciamientos(filter: FuenteFinanciamientoFilter) {
    this.loadingSubject.next(true);
    this.FuenteFinanciamientoService.getFuenteFinanciamientosFilter(
      filter
    ).subscribe((response) => {
      if (response.success) {
        setTimeout(() => {
          response.data.items.forEach((item: FuenteFinanciamiento, index) => {
            item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
          });
          this.FuenteFinanciamientosSubject.next(response.data.items);
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

  connect(
    collectionViewer: CollectionViewer
  ): Observable<FuenteFinanciamiento[]> {
    return this.FuenteFinanciamientosSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.FuenteFinanciamientosSubject.complete();
    this.loadingSubject.complete();
  }
}
