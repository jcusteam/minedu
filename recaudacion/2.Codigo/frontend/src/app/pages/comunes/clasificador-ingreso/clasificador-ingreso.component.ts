import { Component, OnInit, ViewChild } from '@angular/core';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, merge, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CollectionViewer } from '@angular/cdk/collections';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { ClasificadorIngreso, ClasificadorIngresoFilter } from 'src/app/core/models/clasificadoringreso';
import { ClasificadorIngresoService } from 'src/app/core/services/clasificador-ingreso.service';
import { NewClasificadorIngresoComponent } from './dialogs/new-clasificador-ingreso/new-clasificador-ingreso.component';
import { EditClasificadorIngresoComponent } from './dialogs/edit-clasificador-ingreso/edit-clasificador-ingreso.component';
import { InfoClasificadorIngresoComponent } from './dialogs/info-clasificador-ingreso/info-clasificador-ingreso.component';
import { MessageService } from 'src/app/core/services/message.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { Settings } from 'src/app/app.settings.model';
import { Router } from '@angular/router';
import { AppSettings } from 'src/app/app.settings';
import { Accion, Rol, Usuario } from 'src/app/core/models/usuario';
import { AccionEnum, EstadoEnum, MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TransversalService } from 'src/app/core/services/transversal.service';
import { MESSAGES, TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-clasificador-ingreso',
  templateUrl: './clasificador-ingreso.component.html',
  styleUrls: ['./clasificador-ingreso.component.scss']
})
export class ClasificadorIngresoComponent implements OnInit {

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  displayedColumns: string[] = [
    'index',
    'codigo',
    'descripcion',
    'cuentaContableDebe',
    'cuentaContableHaber',
    'estado',
    'actions'
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new ClasificadorIngresoFilter();
  dataSource: ClasificadorIngresoDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private clasificadorIngresoService: ClasificadorIngresoService,
    private transversalService: TransversalService,
    private messageService: MessageService,
    public dialog: MatDialog,
    private authService: AuthService,
    private router: Router,
    public appSettings: AppSettings,
    public fb: FormBuilder,
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      codigo: [null, Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(20)
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
    this.dataSource = new ClasificadorIngresoDataSource(this.clasificadorIngresoService);
    this.onlaodAccion();
    this.dataSource.loadClasificadorIngresos(this.filter);

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
    this.dataSource.loadClasificadorIngresos(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.CLASIFICADOR_INGRESO)
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

    this.filter = new ClasificadorIngresoFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form: ClasificadorIngresoFilter) {

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
    
    let clasificadorIngreso = new ClasificadorIngreso();
    clasificadorIngreso.estado = true;
    clasificadorIngreso.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewClasificadorIngresoComponent, {
      disableClose: true,
      width: '800px',
      data: clasificadorIngreso
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: ClasificadorIngreso): void {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditClasificadorIngresoComponent, {
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

    const dialogRef = this.dialog.open(InfoClasificadorIngresoComponent, {
      hasBackdrop: false,
      width: '800px',
      data: data
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(clasificadorIngreso: ClasificadorIngreso) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        clasificadorIngreso.estado = true;
        clasificadorIngreso.usuarioModificador = this.usuario.numeroDocumento;
        this.clasificadorIngresoService.updateClasificadorIngreso(clasificadorIngreso).subscribe(
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

  onInactivar(clasificadorIngreso: ClasificadorIngreso) {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        clasificadorIngreso.estado = false;
        clasificadorIngreso.usuarioModificador = this.usuario.numeroDocumento;
        this.clasificadorIngresoService.updateClasificadorIngreso(clasificadorIngreso).subscribe(
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

export class ClasificadorIngresoDataSource implements DataSource<ClasificadorIngreso> {
  private ClasificadorIngresosSubject = new BehaviorSubject<ClasificadorIngreso[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;
  constructor(private clasificadorIngresoService: ClasificadorIngresoService) { }

  loadClasificadorIngresos(filter: ClasificadorIngresoFilter) {

    this.loadingSubject.next(true);
    this.clasificadorIngresoService
      .getClasificadorIngresosFilter(filter)
      .subscribe((response) => {
        if (response.success) {
          setTimeout(() => {
            response.data.items.forEach((item: ClasificadorIngreso, index) => {
              item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
            });
            this.ClasificadorIngresosSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<ClasificadorIngreso[]> {
    return this.ClasificadorIngresosSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.ClasificadorIngresosSubject.complete();
    this.loadingSubject.complete();
  }
}
