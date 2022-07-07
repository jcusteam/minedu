import { Component, OnInit } from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { Kardex, KardexSaldo } from "src/app/core/models/kardex";
import { CatalogoBien } from "src/app/core/models/catalogobien";
import { FormBuilder, FormGroup } from "@angular/forms";
import * as FileSaver from "file-saver";

import { CatalogoBienesService } from "src/app/core/services/catalogo-bienes.service";
import { KardexService } from "src/app/core/services/kardex.service";
import { ReporteService } from "src/app/core/services/reporte.service";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { Settings } from "src/app/app.settings.model";
import { Router } from "@angular/router";
import { AppSettings } from "src/app/app.settings";
import { Rol, Accion, Usuario } from "src/app/core/models/usuario";
import { TYPE_MESSAGE } from "src/app/core/utils/messages";
import { AccionEnum, MenuEnum, UnidadEjecturaEnum } from "src/app/core/enums/recaudacion.enum";

@Component({
  selector: "app-kardex",
  templateUrl: "./kardex.component.html",
  styleUrls: ["./kardex.component.scss"],
})
export class KardexComponent implements OnInit {

  //Enumus
  unidadEjecturaEnum = UnidadEjecturaEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "anio",
    "fecha",
    "numeroPecosa",
    "documento",
    "entradaDel",
    "entradaAl",
    "entradaTotal",
    "salidaDel",
    "salidaAl",
    "salidaTotal",
    "saldo",
  ];

  kardexs: Kardex[] = [];
  dataSource = new MatTableDataSource(this.kardexs);
  filteredCatalogoBienes;

  settings: Settings;
  usuario = new Usuario();
  form: FormGroup;

  catalogoBienes: CatalogoBien[] = [];
  acciones: Accion[] = [];

  constructor(
    private catalogoBienService: CatalogoBienesService,
    private kardexService: KardexService,
    private reporteService: ReporteService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router,
    public fb: FormBuilder,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      catalogoBien: [null],
      catalogoBienId: [null]
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    let unidadEjecutoraId = +this.settings.unidadEjecutora;
    if (unidadEjecutoraId != this.unidadEjecturaEnum.UE_024) {
      this.router.navigateByUrl("/");
    }
    // Acciones
    this.onlaodAccion();
    // cargar maestras
    this.loadMaster();

  }


  // Listar Catalogo de bienes
  loadMaster() {
    this.catalogoBienService.getCatalogoBienes().subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.codigoDescripcion = element.codigo + " - " + element.descripcion;
        });

        this.catalogoBienes = response.data;
        this.filteredCatalogoBienes = this.catalogoBienes.slice();
      }
    });
  }

  seletedChangeCatalogo(catalgoBien: CatalogoBien) {
    this.form.patchValue({ catalogoBienId: catalgoBien.catalogoBienId });
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.KARDEX)
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

  // Limpiar formulario
  onClean() {
    this.form.reset();
    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();
  }

  // Buscar
  onSearch() {
    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const catalogoBienId = +this.form.get("catalogoBienId").value;

    if (isNaN(catalogoBienId)) {
      return;
    }

    this.onLoadKardex(catalogoBienId);

  }

  // Entrada Total
  getEntradaTotal(): number {
    return this.dataSource.data.map((t) => t.entradaTotal).reduce((acc, value) => acc + value, 0);
  }

  // Salida Total
  getSalidaTotal(): number {
    return this.dataSource.data.map((t) => t.salidaTotal).reduce((acc, value) => acc + value, 0);
  }

  // Exportar
  exportarKardex() {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    if (this.dataSource.data.length == 0) {
      this.messageService.msgWarning("No hay registros para exportar", () => { });
      return;
    }

    let kardexSaldo = new KardexSaldo();
    let catalogo: CatalogoBien = this.form.get("catalogoBien").value;

    this.messageService.msgLoad("Cargando...");

    kardexSaldo.codigo = catalogo.codigo;
    kardexSaldo.descripcion = catalogo.descripcion;
    kardexSaldo.kardexs = this.dataSource.data;

    var fileName = "Kardex_Almacen_" + kardexSaldo.codigo + ".xlsx";
    this.reporteService.getReporteKardeAlmacen(kardexSaldo).subscribe(
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

  onLoadKardex(catalogoBienId) {
    this.messageService.msgLoad("Cargando...");
    this.kardexService.getKardexs(catalogoBienId).subscribe(
      (response) => {
        this.messageService.msgAutoClose();
        let message = response.messages.join(" , ");
        let data = response.data;
        this.handleResponseKardex(response.messageType, message, response.success, data);
      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  // Respone Kardex
  handleResponseKardex(type, message, success, data: Kardex[]) {
    if (success) {
      this.kardexs = data;
      this.dataSource.data = data;
      this.dataSource._updateChangeSubscription();
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

  // Response Error
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
