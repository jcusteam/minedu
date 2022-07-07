import { GuiaSalidaBienEstado } from './../../../core/models/guiasalidabien';
import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { DatePipe } from "@angular/common";
import {
  GuiaSalidaBien,
  GuiaSalidaBienFilter,
} from "src/app/core/models/guiasalidabien";
import { Estado } from "src/app/core/models/estado";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { MatDialog } from "@angular/material/dialog";
import { GuiaSalidaBienService } from "src/app/core/services/guia-salida-bien.service";
import { EstadoService } from "src/app/core/services/estado.service";
import { NewGuiaSalidaBienComponent } from "./dialogs/new-guia-salida-bien/new-guia-salida-bien.component";
import { InfoGuiaSalidaBienComponent } from "./dialogs/info-guia-salida-bien/info-guia-salida-bien.component";
import { merge, BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { DataSource } from "@angular/cdk/table";
import { CollectionViewer } from "@angular/cdk/collections";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Rol, Accion, Usuario } from "src/app/core/models/usuario";
import { AccionEnum, EstadoGuiaSalidaBienEnum, MenuEnum, TipoDocEnum, UnidadEjecturaEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-guia-salida-bien",
  templateUrl: "./guia-salida-bien.component.html",
  styleUrls: ["./guia-salida-bien.component.scss"],
  providers: [DatePipe],
})
export class GuiaSalidaBienComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enums
  tipoDocEnum = TipoDocEnum;
  estadoGuiaSalidaBienEnum = EstadoGuiaSalidaBienEnum;
  unidadEjecturaEnum = UnidadEjecturaEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "numero",
    "estado",
    "fechaRegistro",
    "justificacion",
    "actions",
  ];

  filter = new GuiaSalidaBienFilter();
  dataSource: GuiaSalidaBienDataSource;
  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Estado[] = [];
  acciones: Accion[] = [];

  constructor(
    public dialog: MatDialog,
    private guiaSalidaBienService: GuiaSalidaBienService,
    private estadoService: EstadoService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public fb: FormBuilder,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      numero: [null, Validators.compose([
        Validators.pattern('^[0-9]+$'),
        Validators.maxLength(12)
      ])],
      estado: [null]
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "guiaSalidaBienId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;

    if (this.filter.unidadEjecutoraId != this.unidadEjecturaEnum.UE_024) {
      this.router.navigateByUrl("/");
    }

    this.dataSource = new GuiaSalidaBienDataSource(this.guiaSalidaBienService);
    this.onlaodAccion();
    this.dataSource.loadGuiaSalidaBien(this.filter);

    this.estadoService.getEstadoByTipoDocumento(this.tipoDocEnum.GUIA_SALIDA_BIEN)
      .subscribe((response) => {
        if (response.success) {
          this.estados = response.data;
        }
      });


  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadGuiaSalidaBienPage()))
      .subscribe();
  }

  loadGuiaSalidaBienPage() {
    this.filter.sortColumn = "guiaSalidaBienId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadGuiaSalidaBien(this.filter);

  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.GUIA_SALIDA)
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
    this.loadGuiaSalidaBienPage();
  }

  onClean() {
    this.filter = new GuiaSalidaBienFilter();
    this.form.reset();
    this.loadGuiaSalidaBienPage();
  }

  // Buscar
  onSearch(form: GuiaSalidaBienFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid)
      return;

    this.filter.numero = form.numero;
    this.filter.estado = form.estado;
    this.paginator.pageIndex = 0;
    this.loadGuiaSalidaBienPage();
  }

  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let guiaSalidaBien = new GuiaSalidaBien();
    guiaSalidaBien.unidadEjecutoraId = +this.settings.unidadEjecutora;
    guiaSalidaBien.estado = this.estadoGuiaSalidaBienEnum.EMITIDO;
    guiaSalidaBien.tipoDocumentoId = this.tipoDocEnum.GUIA_SALIDA_BIEN;
    guiaSalidaBien.usuarioCreador = this.usuario.numeroDocumento;

    const dialogRef = this.dialog.open(NewGuiaSalidaBienComponent, {
      width: "1100px",
      disableClose: false,
      data: guiaSalidaBien,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(guiaSalidaBien): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoGuiaSalidaBienComponent, {
      width: "1100px",
      data: guiaSalidaBien,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onProcess(guiaSalidaBien: GuiaSalidaBien): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_PROCESAR,
      () => {
        this.messageService.msgLoad("Procesando...");
        let guiaSalidaBienEstado = new GuiaSalidaBienEstado();
        guiaSalidaBienEstado.guiaSalidaBienId = guiaSalidaBien.guiaSalidaBienId;
        guiaSalidaBienEstado.estado = this.estadoGuiaSalidaBienEnum.PROCESADO;
        guiaSalidaBienEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.guiaSalidaBienService.updateEstadoGuiaSalidaBien(guiaSalidaBienEstado)
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

  onDelete(guiaSalidaBien: GuiaSalidaBien): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminando...");
        this.guiaSalidaBienService.deleteGuiaSalidaBien(guiaSalidaBien.guiaSalidaBienId)
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

export class GuiaSalidaBienDataSource implements DataSource<GuiaSalidaBien> {
  private GuiaSalidaBiensSubject = new BehaviorSubject<GuiaSalidaBien[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private GuiaSalidaBienService: GuiaSalidaBienService) { }

  loadGuiaSalidaBien(filter: GuiaSalidaBienFilter) {
    this.loadingSubject.next(true);
    this.GuiaSalidaBienService.getGuiaSalidaBienesFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: GuiaSalidaBien, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.GuiaSalidaBiensSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<GuiaSalidaBien[]> {
    return this.GuiaSalidaBiensSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.GuiaSalidaBiensSubject.complete();
    this.loadingSubject.complete();
  }
}
