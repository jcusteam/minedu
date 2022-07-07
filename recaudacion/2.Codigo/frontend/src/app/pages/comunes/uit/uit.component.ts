import { Component, OnInit, ViewChild } from "@angular/core";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, merge, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import { MatDialog } from "@angular/material/dialog";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { Uit, UitFilter } from "src/app/core/models/uit";
import { UitService } from "src/app/core/services/uit.service";
import { NewUitComponent } from "./dialogs/new-uit/new-uit.component";
import { EditUitComponent } from "./dialogs/edit-uit/edit-uit.component";
import { InfoUitComponent } from "./dialogs/info-uit/info-uit.component";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Usuario, Rol, Accion } from "src/app/core/models/usuario";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AccionEnum, EstadoEnum, MenuEnum } from "src/app/core/enums/recaudacion.enum";
import { Combobox } from "src/app/core/interfaces/combobox";
import { TransversalService } from "src/app/core/services/transversal.service";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-uit",
  templateUrl: "./uit.component.html",
  styleUrls: ["./uit.component.scss"],
})
export class UitComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  
  displayedColumns: string[] = [
    "index",
    "periodo",
    "unidadMonetaria",
    "valor",
    "baseLegal",
    "fechaRegistro",
    "estado",
    "actions",
  ];

  //Enums
  menuEnum = MenuEnum;
  eEstado = EstadoEnum;
  accionEnum = AccionEnum;

  filter = new UitFilter();
  dataSource: UitDataSource;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  estados: Combobox[] = [];
  acciones: Accion[] = [];

  constructor(
    private uitService: UitService,
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
      periodo: [null, Validators.compose([
        Validators.pattern('^[0-9]+$'),
        Validators.maxLength(4)
      ])],
      estado: [null]
    });
  }
  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "periodo";
    this.filter.sortOrder = "asc";
    this.dataSource = new UitDataSource(this.uitService);
    this.onlaodAccion();
    this.dataSource.loadUits(this.filter);
    
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
    this.filter.sortColumn = "periodo";
    this.filter.sortOrder = "asc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.dataSource.loadUits(this.filter);
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.UIT)
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

    this.filter = new UitFilter();
    this.form.reset();
    this.loadIntegrantePage();
  }

  onSearch(form?: UitFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (!this.form.valid)
      return;

    this.filter.periodo = form.periodo;
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
    let uit = new Uit();
    uit.estado = true;
    uit.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewUitComponent, {
      disableClose: true,
      width: "800px",
      data: uit
    });
    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: Uit): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditUitComponent, {
      disableClose: true,
      width: "800px",
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

    const dialogRef = this.dialog.open(InfoUitComponent, {
      hasBackdrop: false,
      width: "800px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  onActivar(uit: Uit) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        uit.estado = true;
        uit.usuarioModificador = this.usuario.numeroDocumento;
        this.uitService.updateUit(uit).subscribe(
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

  onInactivar(uit: Uit) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_INACTIVAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        uit.estado = false;
        uit.usuarioModificador = this.usuario.numeroDocumento;
        this.uitService.updateUit(uit).subscribe(
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

export class UitDataSource implements DataSource<Uit> {
  private UitsSubject = new BehaviorSubject<Uit[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private UitService: UitService) { }

  loadUits(filter: UitFilter) {
    this.loadingSubject.next(true);
    this.UitService.getUitsFilter(filter).subscribe((response) => {
      if (response.success) {
        setTimeout(() => {
          response.data.items.forEach((item: Uit, index) => {
            item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
          });
          this.UitsSubject.next(response.data.items);
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

  connect(collectionViewer: CollectionViewer): Observable<Uit[]> {
    return this.UitsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.UitsSubject.complete();
    this.loadingSubject.complete();
  }
}
