import { Component, OnInit, ViewChild } from '@angular/core';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, merge, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CollectionViewer } from '@angular/cdk/collections';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { CatalogoBienesService } from 'src/app/core/services/catalogo-bienes.service';
import { NewCatalogoBienComponent } from './dialogs/new-catalogo-bien/new-catalogo-bien.component';
import { EditCatalogoBienComponent } from './dialogs/edit-catalogo-bien/edit-catalogo-bien.component';
import { InfoCatalogoBienComponent } from './dialogs/info-catalogo-bien/info-catalogo-bien.component';
import { CatalogoBien, CatalogoBienFilter } from 'src/app/core/models/catalogobien';
import { MessageService } from 'src/app/core/services/message.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { Settings } from 'src/app/app.settings.model';
import { AppSettings } from 'src/app/app.settings';
import { Router } from '@angular/router';
import { Usuario, Accion } from 'src/app/core/models/usuario';
import { AccionEnum, EstadoEnum, MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TransversalService } from 'src/app/core/services/transversal.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-catalogo-bien',
  templateUrl: './catalogo-bien.component.html',
  styleUrls: ['./catalogo-bien.component.scss']
})
export class CatalogoBienComponent implements OnInit {

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    'index',
    'codigo',
    'descripcion',
    'clasificadorIngreso',
    'unidadMedida',
    'stockMinimo',
    'puntoReorden',
    'stockMaximo',
    'estado',
    'actions'
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new CatalogoBienFilter();
  dataSource: CatalogoBienDataSource;
  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private catalogoBienService: CatalogoBienesService,
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
      codigo: [null,Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(13)
      ])],
      descripcion: [null, Validators.compose([
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
    this.filter.sortColumn = "codigo";
    this.filter.sortOrder = "asc";
    this.dataSource = new CatalogoBienDataSource(this.catalogoBienService);
    this.onlaodAccion();
    this.dataSource.loadCatalogoBiens(this.filter);
    

    this.transversalService.getEstados().subscribe(
      response => this.estados = response.data
    );

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadCatalogoBienPage()))
      .subscribe();
  }

  loadCatalogoBienPage() {
    this.filter.sortColumn = "codigo";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadCatalogoBiens(this.filter);
  }

  // listar acciones
  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.CATALOGO_BIEN)
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

    this.loadCatalogoBienPage();
  }

  onClean() {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.filter = new CatalogoBienFilter();
    this.form.reset();
    this.loadCatalogoBienPage();
  }

  onSearch(form: CatalogoBienFilter) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if(!this.form.valid){
      return;
    }
    
    this.filter.codigo = form.codigo;
    this.filter.descripcion = form.descripcion;
    this.filter.estado = form.estado;
    this.paginator.pageIndex = 0;
    this.loadCatalogoBienPage();
  }

  // Nuevos
  openDialogNew() {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let catalogoBien = new CatalogoBien();
    catalogoBien.estado = true;
    catalogoBien.usuarioCreador = this.settings.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewCatalogoBienComponent, {
      disableClose: true,
      width: '1000px',
      data: catalogoBien
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: CatalogoBien): void {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.settings.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditCatalogoBienComponent, {
      disableClose: true,
      width: '1000px',
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

    const dialogRef = this.dialog.open(InfoCatalogoBienComponent, {
      disableClose: true,
      width: '1000px',
      data: data
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(catalogoBien: CatalogoBien) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        catalogoBien.estado = true;
        catalogoBien.usuarioModificador = this.settings.usuario.numeroDocumento;
        this.catalogoBienService.updateCatalogoBien(catalogoBien).subscribe(
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

  onInactivar(catalogoBien: CatalogoBien) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        catalogoBien.estado = false;
        catalogoBien.usuarioModificador = this.settings.usuario.numeroDocumento;
        this.catalogoBienService.updateCatalogoBien(catalogoBien).subscribe(
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


export class CatalogoBienDataSource implements DataSource<CatalogoBien> {
  private CatalogoBiensSubject = new BehaviorSubject<CatalogoBien[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private CatalogoBienService: CatalogoBienesService) { }

  loadCatalogoBiens(filter: CatalogoBienFilter) {
    this.loadingSubject.next(true);
    this.CatalogoBienService
      .getCatalogoBienesFilter(filter)
      .subscribe(
        (response) => {
          if (response.success) {
            setTimeout(() => {
              response.data.items.forEach((item: CatalogoBien, index) => {
                item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
              });
              this.CatalogoBiensSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<CatalogoBien[]> {
    return this.CatalogoBiensSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.CatalogoBiensSubject.complete();
    this.loadingSubject.complete();
  }
}
