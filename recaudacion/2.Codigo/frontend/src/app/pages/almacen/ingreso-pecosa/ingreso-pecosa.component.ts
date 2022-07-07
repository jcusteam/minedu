import { IngresoPecosaEstado } from './../../../core/models/ingresopecosa';
import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import {
  IngresoPecosa, IngresoPecosaFilter,
} from "src/app/core/models/ingresopecosa";
import { Estado } from "src/app/core/models/estado";
import { Banco } from "src/app/core/models/banco";
import { Observable, merge, BehaviorSubject } from "rxjs";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { MatDialog } from "@angular/material/dialog";
import { EstadoService } from "src/app/core/services/estado.service";
import { tap } from "rxjs/operators";
import { NewIngresoPecosaComponent } from "./dialogs/new-ingreso-pecosa/new-ingreso-pecosa.component";
import { InfoIngresoPecosaComponent } from "./dialogs/info-ingreso-pecosa/info-ingreso-pecosa.component";
import { DataSource } from "@angular/cdk/table";
import { IngresoPecosaService } from "src/app/core/services/ingreso-pecosa.service";
import { CollectionViewer } from "@angular/cdk/collections";
import { DatePipe } from "@angular/common";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Accion, Usuario } from "src/app/core/models/usuario";
import { AccionEnum, EstadoIngresoPecosaEnum, MenuEnum, TipoDocEnum, UnidadEjecturaEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';


@Component({
  selector: "app-ingreso-pecosa",
  templateUrl: "./ingreso-pecosa.component.html",
  styleUrls: ["./ingreso-pecosa.component.scss"],
  providers: [DatePipe],
})
export class IngresoPecosaComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enums
  tipoDocEnum = TipoDocEnum;
  estadoIngresoPecosaEnum = EstadoIngresoPecosaEnum;
  unidadEjecturaEnum = UnidadEjecturaEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "fechaRegistro",
    "estado",
    "tipoBien",
    "numeroPecosa",
    "nombreAlmacen",
    "motivoPedido",
    "anio",
    "fechaPecosa",
    "actions",
  ];

  filter = new IngresoPecosaFilter();
  dataSource: IngresoPecosasDataSource;
  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Estado[] = [];
  bancos: Banco[] = [];
  acciones: Accion[] = [];

  constructor(
    public dialog: MatDialog,
    private ingresoPecosaService: IngresoPecosaService,
    private estadoService: EstadoService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public appSettings: AppSettings,
    public fb: FormBuilder,
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      numeroPecosa: [null, Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(12)
      ])],
      anioPecosa: [null, Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(4)
      ])],
      estado: [null]
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "ingresoPecosaId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;

    if (this.filter.unidadEjecutoraId != this.unidadEjecturaEnum.UE_024) {
      this.router.navigateByUrl("/");
    }

    this.dataSource = new IngresoPecosasDataSource(this.ingresoPecosaService);
    this.onlaodAccion();
    this.dataSource.loadIngresoPecosas(this.filter);

    this.estadoService.getEstadoByTipoDocumento(this.tipoDocEnum.INGRESO_PECOSA)
      .subscribe((response) => {
        if (response.success) {
          this.estados = response.data;
        }
      });
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadIngresoPecosaPage()))
      .subscribe();
  }

  loadIngresoPecosaPage() {
    this.filter.sortColumn = "ingresoPecosaId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadIngresoPecosas(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.INGRESO_PECOSA)
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
    this.loadIngresoPecosaPage();
  }

  onClean() {
    this.filter = new IngresoPecosaFilter();
    this.form.reset();
    this.loadIngresoPecosaPage();
  }

  // Buscar
  onSearch(form: IngresoPecosaFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid)
      return;

    this.filter.numeroPecosa = form.numeroPecosa;
    this.filter.anioPecosa = form.anioPecosa;
    this.filter.estado = form.estado;
    this.paginator.pageIndex = 0;
    this.loadIngresoPecosaPage();
  }


  openDialogNew(): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let ingresoPecosa = new IngresoPecosa();
    ingresoPecosa.unidadEjecutoraId = +this.settings.unidadEjecutora;
    ingresoPecosa.estado = this.estadoIngresoPecosaEnum.EMITIDO;
    ingresoPecosa.usuarioCreador = this.usuario.numeroDocumento;
    ingresoPecosa.tipoDocumentoId = this.tipoDocEnum.INGRESO_PECOSA;

    const dialogRef = this.dialog.open(NewIngresoPecosaComponent, {
      width: "1100px",
      disableClose: false,
      data: ingresoPecosa,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogInfo(ingresoPecosa): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoIngresoPecosaComponent, {
      width: "1100px",
      data: ingresoPecosa,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onProcess(ingresoPecosa: IngresoPecosa): void {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_PROCESAR,
      () => {
        let ingresoPecosaEstado = new IngresoPecosaEstado();
        ingresoPecosaEstado.ingresoPecosaId = ingresoPecosa.ingresoPecosaId;
        ingresoPecosaEstado.estado = this.estadoIngresoPecosaEnum.PROCESADO;
        ingresoPecosaEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.messageService.msgLoad("Procesando...");
        this.ingresoPecosaService.updateEstadoIngresoPecosa(ingresoPecosaEstado).subscribe(
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

  onDelete(ingresoPecosa: IngresoPecosa): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }


    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminando...");
        this.ingresoPecosaService.deleteIngresoPecosa(ingresoPecosa.ingresoPecosaId).subscribe(
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

export class IngresoPecosasDataSource implements DataSource<IngresoPecosa> {
  private IngresoPecosasSubject = new BehaviorSubject<IngresoPecosa[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private ingresoPecosaService: IngresoPecosaService) { }

  loadIngresoPecosas(filter: IngresoPecosaFilter) {
    this.loadingSubject.next(true);
    this.ingresoPecosaService.getIngresoPecosasFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: IngresoPecosa, index) => {
              item.index =
                (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.IngresoPecosasSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<IngresoPecosa[]> {
    return this.IngresoPecosasSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.IngresoPecosasSubject.complete();
    this.loadingSubject.complete();
  }
}
