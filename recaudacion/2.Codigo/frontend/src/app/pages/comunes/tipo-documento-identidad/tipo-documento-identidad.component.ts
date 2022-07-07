import { Component, OnInit, ViewChild } from '@angular/core';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, merge, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CollectionViewer } from '@angular/cdk/collections';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';

import { TipoDocumentoIdentidadService } from 'src/app/core/services/tipo-documento-identidad.service';
import { NewTipoDocumentoIdentidadComponent } from './dialogs/new-tipo-documento-identidad/new-tipo-documento-identidad.component';
import { EditTipoDocumentoIdentidadComponent } from './dialogs/edit-tipo-documento-identidad/edit-tipo-documento-identidad.component';
import { InfoTipoDocumentoIdentidadComponent } from './dialogs/info-tipo-documento-identidad/info-tipo-documento-identidad.component';
import { TipoDocumentoIdentidad, TipoDocumentoIdentidadFilter } from 'src/app/core/models/tipodocumentoidentidad';
import { MessageService } from 'src/app/core/services/message.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { Settings } from 'src/app/app.settings.model';
import { Router } from '@angular/router';
import { AppSettings } from 'src/app/app.settings';
import { Usuario, Rol, Accion } from 'src/app/core/models/usuario';
import { AccionEnum, EstadoEnum, MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TransversalService } from 'src/app/core/services/transversal.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-tipo-documento-identidad',
  templateUrl: './tipo-documento-identidad.component.html',
  styleUrls: ['./tipo-documento-identidad.component.scss']
})
export class TipoDocumentoIdentidadComponent implements OnInit {

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    'index',
    'codigo',
    'nombre',
    'descripcion',
    'estado',
    'actions'
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new TipoDocumentoIdentidadFilter();
  dataSource: TipoDocumentoIdentidadDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    public dialog: MatDialog,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private transversalService: TransversalService,
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
        Validators.maxLength(30)
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
    this.dataSource = new TipoDocumentoIdentidadDataSource(this.tipoDocumentoIdentidadService);
    this.onlaodAccion();
    this.dataSource.loadTipoDocumentoIdentidads(this.filter);

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
    this.dataSource.loadTipoDocumentoIdentidads(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.TIPO_DOC_IDENTIDAD)
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

    this.filter = new TipoDocumentoIdentidadFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: TipoDocumentoIdentidadFilter) {

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
    let tipoDocumentoIdentidad = new TipoDocumentoIdentidad();
    tipoDocumentoIdentidad.estado = true;
    tipoDocumentoIdentidad.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewTipoDocumentoIdentidadComponent, {
      disableClose: true,
      width: '800px',
      data: tipoDocumentoIdentidad
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: TipoDocumentoIdentidad): void {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditTipoDocumentoIdentidadComponent, {
      disableClose: true,
      width: '800px',
      data: data
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

    const dialogRef = this.dialog.open(InfoTipoDocumentoIdentidadComponent, {
      hasBackdrop: false,
      width: '800px',
      data: data
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(tipoDocumentoIdentidad: TipoDocumentoIdentidad) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        tipoDocumentoIdentidad.estado = true;
        tipoDocumentoIdentidad.usuarioModificador = this.usuario.numeroDocumento;
        this.tipoDocumentoIdentidadService.updateTipoDocumentoIdentidad(tipoDocumentoIdentidad).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {
        this.onRefresh();
      });
  }

  onInactivar(tipoDocumentoIdentidad: TipoDocumentoIdentidad) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        tipoDocumentoIdentidad.estado = false;
        tipoDocumentoIdentidad.usuarioModificador = this.usuario.numeroDocumento;
        this.tipoDocumentoIdentidadService.updateTipoDocumentoIdentidad(tipoDocumentoIdentidad).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      }, () => {
        this.onRefresh();
      });
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


export class TipoDocumentoIdentidadDataSource implements DataSource<TipoDocumentoIdentidad> {
  private TipoDocumentoIdentidadsSubject = new BehaviorSubject<TipoDocumentoIdentidad[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private TipoDocumentoIdentidadService: TipoDocumentoIdentidadService) { }

  loadTipoDocumentoIdentidads(filter: TipoDocumentoIdentidadFilter) {

    this.loadingSubject.next(true);
    this.TipoDocumentoIdentidadService
      .getTipoDocumentoIdentidadesFilter(filter)
      .subscribe((response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: TipoDocumentoIdentidad, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.TipoDocumentoIdentidadsSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<TipoDocumentoIdentidad[]> {
    return this.TipoDocumentoIdentidadsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.TipoDocumentoIdentidadsSubject.complete();
    this.loadingSubject.complete();
  }
}
