import { Component, OnInit, ViewChild } from '@angular/core';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, merge, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CollectionViewer } from '@angular/cdk/collections';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { GrupoRecaudacion, GrupoRecaudacionFilter } from 'src/app/core/models/gruporecaudacion';
import { GrupoRecaudacionService } from 'src/app/core/services/grupo-recaudacion.service';
import { NewGrupoRecaudacionComponent } from './dialogs/new-grupo-recaudacion/new-grupo-recaudacion.component';
import { EditGrupoRecaudacionComponent } from './dialogs/edit-grupo-recaudacion/edit-grupo-recaudacion.component';
import { InfoGrupoRecaudacionComponent } from './dialogs/info-grupo-recaudacion/info-grupo-recaudacion.component';
import { MessageService } from 'src/app/core/services/message.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { Settings } from 'src/app/app.settings.model';
import { Router } from '@angular/router';
import { AppSettings } from 'src/app/app.settings';
import { Usuario, Rol, Accion } from 'src/app/core/models/usuario';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccionEnum, EstadoEnum, MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { TransversalService } from 'src/app/core/services/transversal.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-grupo-recaudacion',
  templateUrl: './grupo-recaudacion.component.html',
  styleUrls: ['./grupo-recaudacion.component.scss']
})
export class GrupoRecaudacionComponent implements OnInit {

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    'index',
    'nombre',
    'estado',
    'actions'
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;
  
  filter = new GrupoRecaudacionFilter();
  dataSource: GrupoRecaudacionDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private grupoRecaudacionService: GrupoRecaudacionService,
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
        Validators.maxLength(100)
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
    this.dataSource = new GrupoRecaudacionDataSource(this.grupoRecaudacionService);
    this.onlaodAccion();
    this.dataSource.loadGrupoRecaudacions(this.filter);
    
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
    this.dataSource.loadGrupoRecaudacions(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.GRUPO_RECAUDACION)
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

    this.filter = new GrupoRecaudacionFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form?: GrupoRecaudacionFilter) {

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
    let grupoRecaudacion = new GrupoRecaudacion();
    grupoRecaudacion.estado = true;
    grupoRecaudacion.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewGrupoRecaudacionComponent, {
      disableClose: true,
      width: '800px',
      data: grupoRecaudacion
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: GrupoRecaudacion): void {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditGrupoRecaudacionComponent, {
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

    const dialogRef = this.dialog.open(InfoGrupoRecaudacionComponent, {
      hasBackdrop: false,
      width: '800px',
      data: data
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(grupoRecaudacion: GrupoRecaudacion) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        grupoRecaudacion.estado = true;
        grupoRecaudacion.usuarioModificador = this.usuario.numeroDocumento;
        this.grupoRecaudacionService.updateGrupoRecaudacion(grupoRecaudacion).subscribe(
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

  onInactivar(grupoRecaudacion: GrupoRecaudacion) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        grupoRecaudacion.estado = false;
        grupoRecaudacion.usuarioModificador = this.usuario.numeroDocumento;
        this.grupoRecaudacionService.updateGrupoRecaudacion(grupoRecaudacion).subscribe(
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


export class GrupoRecaudacionDataSource implements DataSource<GrupoRecaudacion> {
  private GrupoRecaudacionsSubject = new BehaviorSubject<GrupoRecaudacion[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private GrupoRecaudacionService: GrupoRecaudacionService) { }

  loadGrupoRecaudacions(filter: GrupoRecaudacionFilter) {

    this.loadingSubject.next(true);
    this.GrupoRecaudacionService
      .getGrupoRecaudacionesFilter(filter)
      .subscribe((response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: GrupoRecaudacion, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.GrupoRecaudacionsSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<GrupoRecaudacion[]> {
    return this.GrupoRecaudacionsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.GrupoRecaudacionsSubject.complete();
    this.loadingSubject.complete();
  }
}
