import { ReciboIngresoEstado } from './../../../../core/models/reciboingreso';
import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { MatSort } from "@angular/material/sort";
import { MatPaginator } from "@angular/material/paginator";
import { Router } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { tap } from "rxjs/operators";
import { CollectionViewer } from "@angular/cdk/collections";
import * as FileSaver from "file-saver";

import {
  ReporteLiquidacionIngreso,
  ReciboIngreso,
  ReciboIngresoFilter,
  ReporteReciboIngreso,
  ReporteReciboIngresoDetalle,
} from "src/app/core/models/reciboingreso";
import { ReciboIngresoService } from "src/app/core/services/recibo-ingreso.service";
import { TipoReciboIngreso } from "src/app/core/models/tiporeciboingreso";
import { TipoReciboIngresoService } from "src/app/core/services/tipo-recibo-ingreso.service";
import { InfoReciboIngresoComponent } from "../dialogs/info-recibo-ingreso/info-recibo-ingreso.component";

import { NewReciboIngresoComponent } from "../dialogs/new-recibo-ingreso/new-recibo-ingreso.component";
import { Estado } from "src/app/core/models/estado";
import { EstadoService } from "src/app/core/services/estado.service";
import { ClienteService } from "src/app/core/services/cliente.service";
import { EditReciboIngresoComponent } from "../dialogs/edit-recibo-ingreso/edit-recibo-ingreso.component";
import { TipoCaptacion } from "src/app/core/models/tipocaptacion";
import { TipoCaptacionService } from "src/app/core/services/tipo-captacion.service";
import { TipoDocumentoIdentidadService } from "src/app/core/services/tipo-documento-identidad.service";
import { TipoDocumentoIdentidad } from "src/app/core/models/tipodocumentoidentidad";
import { UnidadEjecutoraService } from "src/app/core/services/unidad-ejecutora.service";
import { LiquidacionService } from "src/app/core/services/liquidacion.service";
import { ReporteService } from "src/app/core/services/reporte.service";
import { MessageService } from "src/app/core/services/message.service";
import { AuthService } from "src/app/core/services/auth.service";
import { AppSettings } from "src/app/app.settings";
import { Settings } from "src/app/app.settings.model";
import { Usuario, Accion } from "src/app/core/models/usuario";
import { AccionEnum, EstadoReciboIngresoEnum, MenuEnum, TipoCaptacionEnum, TipoDocEnum, TipoDocIdentidadEnum, ValidaDepositoEnum } from "src/app/core/enums/recaudacion.enum";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { UnidadEjecutora } from "src/app/core/models/unidadejecutora";
import { MESSAGES, TYPE_MESSAGE } from "src/app/core/utils/messages";

@Component({
  selector: "app-recibo-ingreso",
  templateUrl: "./recibo-ingreso.component.html",
  styleUrls: ["./recibo-ingreso.component.scss"],
})
export class ReciboIngresoComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  //Enums
  tipoDocEnum = TipoDocEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoCaptacionEnum = TipoCaptacionEnum;
  estadoReciboIngreso = EstadoReciboIngresoEnum;
  validaDepositoEnum = ValidaDepositoEnum;
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  displayedColumns: string[] = [
    "index",
    "numero",
    "fechaEmision",
    "estado",
    "tipoDocumentoCliente",
    "nroDocumento",
    "nombre",
    "tipoRecibo",
    "fuenteFinanciamiento",
    "tipoCaptacion",
    "nroCuentaCorriente",
    "importeTotal",
    "nroDeposito",
    "fechaDeposito",
    "validarDeposito",
    "nroCheque",
    "registroLinea",
    "registroSinad",
    "nroOficio",
    "cartaOrden",
    "compPago",
    "expedienteSiaf",
    "nroResolucion",
    "correo",
    "actions",
  ];

  filter = new ReciboIngresoFilter();
  dataSource: ReciboIngresosDataSource;

  settings: Settings;
  usuario = new Usuario();


  //estados
  estados: Estado[] = [];
  //Tipo de captaciones
  tipoCaptaciones: TipoCaptacion[] = [];
  //Tipo de recibos de ingresos
  tipoReciboIngresos: TipoReciboIngreso[] = [];
  //Tipo de documentos de identidad del cliente
  tipoDocumentoClientes: TipoDocumentoIdentidad[] = [];
  // Acciones
  acciones: Accion[] = [];

  form: FormGroup;

  unidadEjecutora = new UnidadEjecutora();

  constructor(
    public fb: FormBuilder,
    public reciboIngresoService: ReciboIngresoService,
    public router: Router,
    public dialog: MatDialog,
    public appSettings: AppSettings,
    private tipoReciboIngresoServie: TipoReciboIngresoService,
    private estadoService: EstadoService,
    private tipoCaptacionService: TipoCaptacionService,
    private clienteService: ClienteService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private unidadEjecutoraService: UnidadEjecutoraService,
    private liquidacionService: LiquidacionService,
    private reporteService: ReporteService,
    private messageService: MessageService,
    private authService: AuthService
  ) {
    this.settings = this.appSettings.settings;
    this.form = this.fb.group({
      tipoDocumentoIdentidadId: [null],
      tipoReciboIngresoId: [null],
      tipoCaptacionId: [null],
      numeroDocumento: [null, Validators.compose([
        Validators.pattern("^[a-zA-Z0-9]+$"),
        Validators.maxLength(12)
      ])],
      clienteId: [null],
      clienteNombre: [null],
      estado: [null],
    });
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.filter.pageNumber = 1;
    this.filter.pageSize = 10;
    this.filter.sortColumn = "reciboIngresoId";
    this.filter.sortOrder = "desc";
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource = new ReciboIngresosDataSource(this.reciboIngresoService);
    this.onlaodAccion();
    this.dataSource.loadReciboIngresos(this.filter);

    this.loadMaestras();

  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadReciboIngresoPage()))
      .subscribe();
  }

  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.RECIBO_INGRESO)
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

  loadMaestras() {

    // Unidad Ejecutora
    this.unidadEjecutoraService.getUnidadEjecutoraById(this.filter.unidadEjecutoraId).subscribe(
      response => {
        if (response.success) {
          this.unidadEjecutora = response.data;
        }
      }
    );

    // Tipo Recibo  Ingresos
    this.tipoReciboIngresoServie
      .getTipoReciboIngresos()
      .subscribe((response) => {
        if (response.success) {
          this.tipoReciboIngresos = response.data;
        }
      });

    // Estado de Documentos
    this.estadoService
      .getEstadoByTipoDocumento(this.tipoDocEnum.RECIBO_INGRESO)
      .subscribe((response) => {
        if (response.success) {
          this.estados = response.data;
        }
      });

    this.tipoCaptacionService.getTipoCaptaciones().subscribe((response) => {
      if (response.success) {
        this.tipoCaptaciones = response.data;
      }
    });

    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService.getTipoDocumentoIdentidades().subscribe((response) => {
      if (response.success) {
        this.tipoDocumentoClientes = response.data;
      }
    });


  }

  onSelectionTipoDocIdentidad(id: number) {
    this.filter.tipoDocumentoIdentidadId = id;
    this.form.patchValue({
      numeroDocumento: null,
      clienteNombre: null,
    });

    this.valiationTipoDocIdentidad(id);
  }

  valiationTipoDocIdentidad(id: number) {
    if (id == this.tipoDocIdentidadEnum.DNI) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{8,8}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.CE) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    } else if (id == this.tipoDocIdentidadEnum.RUC) {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[0-9]{11,11}$"),
        ]);
      this.form.updateValueAndValidity();
    } else {
      this.form.get("numeroDocumento")
        .setValidators([
          Validators.required,
          Validators.pattern("^[a-zA-Z0-9]{8,12}$"),
        ]);
      this.form.updateValueAndValidity();
    }
  }

  searchNroDoc() {

    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.form.patchValue({
      clienteNombre: null,
    });

    const tipoDocIdentidadId = +this.form.get("tipoDocumentoIdentidadId").value;
    const numeroDocumento = this.form.get("numeroDocumento").value;

    if (isNaN(tipoDocIdentidadId)) {
      return;
    }

    if (!this.form.get("numeroDocumento").valid || numeroDocumento == null) {
      return;
    }

    this.clienteService.getClienteByTipoNroDocumento(tipoDocIdentidadId, numeroDocumento).subscribe(
      (response) => {
        var message = response.messages.join(",");
        if (response.success) {
          let cliente = response.data;
          this.form.patchValue({ clienteNombre: cliente.nombre, clienteId: cliente.clienteId })
        } else {
          if (response.messageType == TYPE_MESSAGE.INFO) {
            this.messageService.msgInfo(message, () => { });
          } else if (response.messageType == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { });
          } else {
            this.messageService.msgError(message, () => { });
          }
        }
      },
      (error) => this.handleError(error)
    );
  }

  loadReciboIngresoPage() {
    this.filter.sortColumn = "reciboIngresoId";
    this.filter.sortOrder = "desc";
    this.filter.pageNumber = this.paginator.pageIndex + 1;
    this.filter.pageSize = this.paginator.pageSize;
    this.filter.unidadEjecutoraId = +this.settings.unidadEjecutora;
    this.dataSource.loadReciboIngresos(this.filter);
  }

  onRefresh() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.loadReciboIngresoPage();
  }

  onClean() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.form.reset();
    this.filter = new ReciboIngresoFilter();
    this.form.get("numeroDocumento").clearValidators();
    this.form.get("numeroDocumento").updateValueAndValidity();
    this.loadReciboIngresoPage();
  }

  onSearch(form: ReciboIngresoFilter) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.filter.clienteId = form.clienteId;
    this.filter.tipoReciboIngresoId = form.tipoReciboIngresoId;
    this.filter.tipoCaptacionId = form.tipoCaptacionId;
    this.filter.estado = form.estado;

    this.loadReciboIngresoPage();
  }

  openDialogInfo(data): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    const dialogRef = this.dialog.open(InfoReciboIngresoComponent, {
      width: "1000px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogEdit(data: ReciboIngreso): void {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    data.usuarioModificador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(EditReciboIngresoComponent, {
      width: "1000px",
      data: data,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openDialogNew() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let reciboIngreso = new ReciboIngreso();
    reciboIngreso.estado = this.estadoReciboIngreso.EMITIDO;
    reciboIngreso.unidadEjecutoraId = +this.settings.unidadEjecutora;
    reciboIngreso.tipoDocumentoId = this.tipoDocEnum.RECIBO_INGRESO;
    reciboIngreso.usuarioCreador = this.usuario.numeroDocumento;
    const dialogRef = this.dialog.open(NewReciboIngresoComponent, {
      width: "1200px",
      data: reciboIngreso,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.onRefresh();
    });
  }

  openProcess(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.CONFIRM_PROCESS,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.PROCESADO;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
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

  openConfirm(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.CONFIRM,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.CONFIRMADO;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
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

  openEnvioSiaf(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.CONFIRM_ENVIO_SIAF,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.ENVIO_SIAF;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
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

  openTransmitir(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.CONFIRM_TRANSMITIR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.TRANSMITIDO;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
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

  openRechazar(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.CONFIRM_RECHAZAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.RECHAZADO;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
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

  openAnular(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.CONFIRM_ANULAR,
      () => {
        this.messageService.msgLoad("Actualizando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.ANULADO;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      },
      () => { this.onRefresh() }
    );
  }

  openAnularPost(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.RECIBO_INGRESO.ONFIRM_ANULAR_POSTERIOR,
      () => {
        this.messageService.msgLoad("Anulando...");
        let reciboIngresoEstado = new ReciboIngresoEstado();
        reciboIngresoEstado.reciboIngresoId = data.reciboIngresoId;
        reciboIngresoEstado.estado = this.estadoReciboIngreso.ANULADO_POSTERIOR;
        reciboIngresoEstado.usuarioModificador = this.usuario.numeroDocumento;
        this.reciboIngresoService.updateEstadoReciboIngreso(reciboIngresoEstado).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      },
      () => { this.onRefresh() }
    );
  }

  openDelete(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    this.messageService.msgConfirm(MESSAGES.FORM.CONFIRM_ELIMINAR,
      () => {
        this.messageService.msgLoad("Eliminado...");
        this.reciboIngresoService.deleteReciboIngreso(data.reciboIngresoId).subscribe(
          (response) => {
            var message = response.messages.join(",");
            this.handleResponse(response.messageType, message, response.success);
          },
          (error) => this.handleError(error)
        );
      },
      () => { this.onRefresh() }
    );
  }

  openReport(data: ReciboIngreso) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }
    let reporte = new ReporteReciboIngreso();
    let liquidacionIngresos: ReporteLiquidacionIngreso[] = [];

    let detalles: ReporteReciboIngresoDetalle[] = [];
    this.messageService.msgLoad("Generando...");



    let unidadEjecutora = this.unidadEjecutora;
    reporte.ejecutora = "U.E " + unidadEjecutora?.codigo.trim() + " - " + unidadEjecutora.nombre;
    reporte.secEje = unidadEjecutora?.secuencia.trim();
    reporte.numeroRuc = unidadEjecutora?.numeroRuc.trim();
    reporte.numero = data.numero;
    reporte.fecha = data.fechaEmision;
    reporte.procedencia = data.cliente.nombre;
    reporte.glosa = data.tipoReciboIngreso.nombre;
    reporte.cuentaCorriente = "CTA. CTE N° " + data.cuentaCorriente.numero + " " + data.cuentaCorriente.denominacion;
    reporte.concepto = data.concepto;

    reporte.numeroComprobantePago = data.numeroComprobantePago;
    reporte.expedienteSiaf = data.expedienteSiaf;
    reporte.numeroResolucion = data.numeroResolucion;
    reporte.numeroDeposito = data.numeroDeposito;

    reporte.total = data.importeTotal;
    reporte.parcial = data.importeTotal;

    reporte.pliego = "010";
    reporte.fuenteFinanciamiento = "";
    reporte.unidad = unidadEjecutora?.codigo.trim();
    reporte.codigo = "";
    reporte.codigoDos = "";
    reporte.cuentaDebe = "";
    reporte.boletaVenta = "";
    reporte.factura = "";
    if (data.tipoCaptacionId == this.tipoCaptacionEnum.VARIOS) {
      this.liquidacionService.getLiquidacionById(data.liquidacionId).subscribe(
        (response) => {
          var message = response.messages.join(" , ");
          if (response.success) {
            reporte.boletaVenta = response.data.boletaVenta;
            reporte.factura = response.data.factura;
            reporte.numeroLiquidacion = response.data.numero;
            response.data.liquidacionDetalle.forEach((item) => {
              let detalle = new ReporteLiquidacionIngreso();
              detalle.clasificadorIngresoId = item.clasificadorIngresoId;
              detalle.codigo = item.clasificadorIngreso?.codigo;
              detalle.nombre = item.clasificadorIngreso?.descripcion.toUpperCase();
              detalle.nombreTipoCaptacion = item?.tipoCaptacion?.nombre.toUpperCase();
              detalle.total = item.importeParcial;
              liquidacionIngresos.push(detalle);
            });
            reporte.liquidaciones = liquidacionIngresos;
            var fileName = "ReciboIngreso " + data.tipoReciboIngreso.nombre + ".pdf";
            this.onDownladReporteRIVentanilla(reporte, fileName);
          } else {
            if (response.messageType == TYPE_MESSAGE.INFO) {
              this.messageService.msgInfo(message, () => { });
            } else if (response.messageType == TYPE_MESSAGE.WARNING) {
              this.messageService.msgWarning(message, () => { });
            } else {
              this.messageService.msgError(message, () => { });
            }

          }
        },
        (error) => {
          this.handleError(error)
        }
      );
    }
    else {
      this.reciboIngresoService.getReciboIngresoById(data.reciboIngresoId).subscribe(
        (response) => {
          var message = response.messages.join(" , ");
          if (response.success) {
            response.data.reciboIngresoDetalle.forEach(element => {
              let detalle = new ReporteReciboIngresoDetalle();
              let clasificadorIngreso = element.clasificadorIngreso;
              detalle.clasificador = clasificadorIngreso?.codigo + "   " + clasificadorIngreso?.descripcion;
              detalle.parcial = element.importe;
              detalles.push(detalle);
            });
            reporte.detalles = detalles;
            var fileName = "ReciboIngreso " + data.tipoReciboIngreso.nombre + ".pdf";
            console.log(JSON.stringify(reporte))
            this.onDownladReporteRI(reporte, fileName);
          }
          else {
            if (response.messageType == TYPE_MESSAGE.INFO) {
              this.messageService.msgInfo(message, () => { });
            } else if (response.messageType == TYPE_MESSAGE.WARNING) {
              this.messageService.msgWarning(message, () => { });
            } else {
              this.messageService.msgError(message, () => { });
            }
          }
        },
        (error) => {
          this.handleError(error);
        }
      );
    }


  }

  onDownladReporteRI(data, fileName) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.reporteService.getReporteReciboIngreso(data).subscribe(
      (file: Blob) => {
        setTimeout(() => {
          this.messageService.msgAutoClose();
          FileSaver.saveAs(file, fileName);
        }, 500);
      },
      (error) => {
        this.handleError(error)
      }
    );
  }

  onDownladReporteRIVentanilla(data, fileName) {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.reporteService.getReporteReciboIngresoVentanilla(data).subscribe(
      (file: Blob) => {
        setTimeout(() => {
          this.messageService.msgAutoClose();
          FileSaver.saveAs(file, fileName);
        }, 500);
      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  // Response
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

export class ReciboIngresosDataSource implements DataSource<ReciboIngreso> {
  private reciboIngresosSubject = new BehaviorSubject<ReciboIngreso[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public totalItems = 0;
  public loading$ = this.loadingSubject.asObservable();
  public isLoadingResults = true;

  constructor(private ReciboIngresoService: ReciboIngresoService) { }

  loadReciboIngresos(filter: ReciboIngresoFilter) {
    this.loadingSubject.next(true);
    this.ReciboIngresoService.getReciboIngresosFilter(filter).subscribe(
      (response) => {
        if (response.success) {
          response.data.items.forEach((item: ReciboIngreso, index) => {
            item.index = (filter.pageNumber - 1) * filter.pageSize + (index + 1);
          });
          this.reciboIngresosSubject.next(response.data.items);
          this.totalItems = response.data.total;
          this.isLoadingResults = false;
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

  connect(collectionViewer: CollectionViewer): Observable<ReciboIngreso[]> {
    return this.reciboIngresosSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.reciboIngresosSubject.complete();
    this.loadingSubject.complete();
  }
}
