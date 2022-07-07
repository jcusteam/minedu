import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { CatalogoBienSaldoService } from 'src/app/core/services/catalogo-bien-saldo.service';
import { tap } from 'rxjs/operators';
import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, merge } from 'rxjs';
import { CollectionViewer } from '@angular/cdk/collections';
import * as FileSaver from "file-saver";
import { ReporteService } from 'src/app/core/services/reporte.service';
import { CatalogoBienFilter, CatalogoBienSaldo } from 'src/app/core/models/catalogobien';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { Settings } from 'src/app/app.settings.model';
import { AppSettings } from 'src/app/app.settings';
import { Router } from '@angular/router';
import { Accion, Usuario } from 'src/app/core/models/usuario';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TYPE_MESSAGE } from 'src/app/core/utils/messages';
import { AccionEnum, MenuEnum, UnidadEjecturaEnum } from 'src/app/core/enums/recaudacion.enum';

@Component({
  selector: 'app-saldo',
  templateUrl: './saldo.component.html',
  styleUrls: ['./saldo.component.scss']
})
export class SaldoComponent implements OnInit, AfterViewInit {
  
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enumus
  unidadEjecturaEnum = UnidadEjecturaEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    'index',
    'codigo',
    'descripcion',
    'unidadMedida',
    'stockMinimo',
    'puntoReorden',
    'stockMaximo',
    'saldo'
  ];

  filter = new CatalogoBienFilter();
  dataSource: CatalogoBienSaldoDataSource;
  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  acciones: Accion[] = [];

  constructor(
    private catalogoBienSaldoService: CatalogoBienSaldoService,
    private reporteService: ReporteService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public fb: FormBuilder,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      codigo: [null, Validators.compose([
        Validators.pattern("^[0-9]+$"),
        Validators.maxLength(13)
      ])],
      descripcion: [null, Validators.compose([
        Validators.pattern('^[A-Za-zÀ-ÿñÑ0-9#°().,_ -]+$'),
        Validators.maxLength(200)
      ])]
    });
  }


  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "nombre";
    this.filter.sortOrder = "asc";
    this.filter.estado = true;

    let unidadEjecutoraId =  +this.settings.unidadEjecutora;
    if(unidadEjecutoraId != this.unidadEjecturaEnum.UE_024){
      this.router.navigateByUrl("/");
    }

    this.dataSource = new CatalogoBienSaldoDataSource(this.catalogoBienSaldoService);
    this.onlaodAccion();
    this.dataSource.loadCatalogoBiens(this.filter);

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadCatalogoBienPage()))
      .subscribe();
  }

  loadCatalogoBienPage() {
    this.filter.sortColumn = "nombre";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.estado = true;
    this.dataSource.loadCatalogoBiens(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.SALDO_ALMACEN)
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

    if (!this.form.valid) {
      return;
    }

    this.filter.codigo = form.codigo;
    this.filter.descripcion = form.descripcion;
    this.paginator.pageIndex = 0;
    this.loadCatalogoBienPage();
  }

  exportar() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (this.dataSource.totalItems == 0) {
      return;
    }

    this.messageService.msgLoad("Exportando...");
    this.catalogoBienSaldoService.getCatalogoBienSaldos().subscribe(
      (response) => {
        var message = response.messages.join(",");
        this.handleResponseCatalogoSaldo(response.messageType, message, response.success, response.data);
      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  // Respone
  handleResponseCatalogoSaldo(type, message, success, data) {
    if (success) {
      let saldoAlmacen = {
        catalogoBienes: data
      };
      this.onGenerateExecel(saldoAlmacen);
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { });
      }
      else {
        this.messageService.msgError(message, () => { });
      }
    }
  }

  onGenerateExecel(data) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    let fileName = "Saldo Almacen.xlsx"
    this.reporteService.getReporteSaldoAlmacen(data).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        if (response) {
          FileSaver.saveAs(response, fileName);
        }


      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  // Response
  handleResponse(type, message, success) {
    if (success) {
      this.messageService.msgSuccess(message, () => { });
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { });
      }
      else {
        this.messageService.msgError(message, () => { });
      }
    }
  }

  // Errror
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


export class CatalogoBienSaldoDataSource implements DataSource<CatalogoBienSaldo> {
  private CatalogoBienesSaldoSubject = new BehaviorSubject<CatalogoBienSaldo[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private catalogoBienSaldoService: CatalogoBienSaldoService) { }

  loadCatalogoBiens(filter: CatalogoBienFilter) {
    this.loadingSubject.next(true);
    this.catalogoBienSaldoService
      .getCatalogoBienSaldosFilter(filter)
      .subscribe(
        (response) => {
          if (response.success) {
            setTimeout(() => {
              response.data.items.forEach((item: CatalogoBienSaldo, index) => {
                item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
              });
              this.CatalogoBienesSaldoSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<CatalogoBienSaldo[]> {
    return this.CatalogoBienesSaldoSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.CatalogoBienesSaldoSubject.complete();
    this.loadingSubject.complete();
  }

}
