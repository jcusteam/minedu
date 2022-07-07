import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { MatTableDataSource } from '@angular/material/table';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { TipoDocEnum, ValidaDepositoEnum, TipoDocIdentidadEnum, TipoAdquisicionEnum, TipoCaptacionEnum, TipoMonedaEnum, TipoOperacionEnum, TipoIGVEnum, IGVEnum, TipoComprobanteEnum, FuenteOrigenEnum, FuenteValidaEnum } from 'src/app/core/enums/recaudacion.enum';
import { Combobox } from 'src/app/core/interfaces/combobox';
import { CatalogoBien } from 'src/app/core/models/catalogobien';
import { ComprobantePagoDetalle, ComprobantePago } from 'src/app/core/models/comprobantepago';
import { CuentaCorriente } from 'src/app/core/models/cuentacorriente';
import { Tarifario } from 'src/app/core/models/tarifario';
import { TipoCaptacion } from 'src/app/core/models/tipocaptacion';
import { TipoComprobantePago } from 'src/app/core/models/tipocomprobantepago';
import { TipoDocumentoIdentidad } from 'src/app/core/models/tipodocumentoidentidad';
import { CatalogoBienesService } from 'src/app/core/services/catalogo-bienes.service';
import { ComprobantePagoService } from 'src/app/core/services/comprobante-pago.service';
import { CuentaCorrienteService } from 'src/app/core/services/cuenta-corriente.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TarifarioService } from 'src/app/core/services/tarifario.service';
import { TipoCaptacionService } from 'src/app/core/services/tipo-captacion.service';
import { TipoComprobantePagoService } from 'src/app/core/services/tipo-comprobante-pago.service';
import { TipoDocumentoIdentidadService } from 'src/app/core/services/tipo-documento-identidad.service';
import { TransversalService } from 'src/app/core/services/transversal.service';
import { MESSAGES } from 'src/app/core/utils/messages';
import { Tools } from 'src/app/core/utils/tools';


const DATE_MODE_FORMATS = {
  parse: {
    dateInput: "DD/MM/YYYY",
  },
  display: {
    dateInput: "DD/MM/YYYY",
    monthYearLabel: "MMM YYYY",
    dateA11yLabel: "LL",
    monthYearA11yLabel: "MMMM YYYY",
  },
};

@Component({
  selector: 'app-info-factura',
  templateUrl: './info-factura.component.html',
  styleUrls: ['./info-factura.component.scss'],
  providers: [
    { provide: DatePipe },
    { provide: MAT_DATE_LOCALE, useValue: "es-ES" },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: DATE_MODE_FORMATS },
  ]
})
export class InfoFacturaComponent implements OnInit {
  
  // Enums
  tipoDocEnum = TipoDocEnum;
  validaDepositoEnum = ValidaDepositoEnum;
  tipoDocIdentidadEnum = TipoDocIdentidadEnum;
  tipoAdquisicionEnum = TipoAdquisicionEnum;
  tipoCaptacionEnum = TipoCaptacionEnum;
  tipoMonedaEnum = TipoMonedaEnum;
  tipoOperacionEnum = TipoOperacionEnum;
  tipoIGVEnum = TipoIGVEnum;
  IGVEnum = IGVEnum;
  tipoComprobanteEnum = TipoComprobanteEnum;
  fuenteOrigenEnum = FuenteOrigenEnum;
  fuenteValidaEnum = FuenteValidaEnum;

  // Tarifarios
  tarifarios: Tarifario[] = [];
  filteredTarifarios;

  // Catalogo de bienes
  catalogoBienes: CatalogoBien[] = [];
  filteredCatalogoBienes;

  // Cuentas Corrientes
  cuentasCorrientes: CuentaCorriente[] = [];
  filteredCuentasCorrientes;

  // Tipo Doc Identidad
  tipoDocIdentidades: TipoDocumentoIdentidad[] = [];
  tipoDocIdentidadesEncargado: TipoDocumentoIdentidad[] = [];
  tipoFuentes: TipoComprobantePago[] = [];

  hiddenDeposito = true;
  hiddenCheque = true;
  hiddenCatalogo = true;
  hiddenTarifario = false;
  hiddenDocFuente = true;
  hiddenValidaFuente = true;

  // Combobox
  tipoOperaciones: Combobox[] = [];
  tipoMonedas: Combobox[] = [];
  tipoCaptaciones: TipoCaptacion[] = [];
  tipoAdquisiciones: Combobox[] = [];
  tipoIgvs: Combobox[] = [];
  tipoCondicionPagos: Combobox[] = [];
  tipoNotaCreditos: Combobox[] = [];
  tipoNotaDebitos: Combobox[] = [];
  fuenteOrigenes: Combobox[] = [];

  // Nombre de Columna de la tabla
  displayedColumns: string[] = [
    "nro",
    "descripcion",
    "cantidad",
    "precio",
    "descuento",
    "subTotal",
    "valorVenta"
  ];

  // Listado detalle Comprobante 
  comprobantePagoDetalles: ComprobantePagoDetalle[] = [];

  // Data Source
  dataSource = new MatTableDataSource(this.comprobantePagoDetalles);

  // Fecha
  minDate = new Date();
  maxDate = new Date();

  // Form
  form: FormGroup;

  // Constructor
  constructor(
    public dialogRef: MatDialogRef<InfoFacturaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ComprobantePago,
    public dialog: MatDialog,
    private fb: FormBuilder,
    private catalogoBienService: CatalogoBienesService,
    private tarifarioService: TarifarioService,
    private tipoCaptacionService: TipoCaptacionService,
    private tipoComprobantePagoService: TipoComprobantePagoService,
    private tipoDocumentoIdentidadService: TipoDocumentoIdentidadService,
    private cuentaCorrienteService: CuentaCorrienteService,
    private comprobantePagoService: ComprobantePagoService,
    private messageService: MessageService,
    private transversalService: TransversalService
  ) {

    // Formulario 
    this.form = this.fb.group({
      comprobantePagoId: [data?.comprobantePagoId],
      clienteId: [data?.clienteId],
      unidadEjecutoraId: [data?.unidadEjecutoraId],
      tipoComprobanteId: [data?.tipoComprobanteId],
      tipoDocumentoId: [data?.tipoDocumentoId],
      depositoBancoDetalleId: [data?.depositoBancoDetalleId],
      cuentaCorrienteId: [data?.cuentaCorrienteId],
      tipoCaptacionId: [data?.tipoCaptacionId],
      comprobanteEmisor: [data?.comprobanteEmisorId],
      serie: [data?.serie],
      correlativo: [data?.correlativo],
      fechaEmision: [data?.fechaEmision],
      fechaVencimiento: [data?.fechaVencimiento],
      tipoAdquisicion: [data?.tipoAdquisicion],
      codigoTipoOperacion: [data?.codigoTipoOperacion],
      tipoCondicionPago: [data?.tipoCondicionPago],
      numeroDeposito: [data?.numeroDeposito],
      fechaDeposito: [data?.fechaDeposito],
      validarDeposito: [data?.validarDeposito],
      numeroCheque: [data?.numeroCheque],
      encargadoTipoDocumento: [data?.encargadoTipoDocumento],
      encargadoNombre: [data?.encargadoNombre],
      encargadoNumeroDocumento: [data?.encargadoNumeroDocumento],
      fuenteId: [data?.fuenteId],
      fuenteTipoDocumento: [data?.fuenteTipoDocumento],
      fuenteSerie: [data?.fuenteSerie],
      fuenteCorrelativo: [data?.fuenteCorrelativo],
      fuenteOrigen: [data?.fuenteOrigen],
      fuenteValidar: [data?.fuenteValidar],
      sustento: [data?.sustento],
      observacion: [data?.observacion],
      nombreArchivo: [data?.nombreArchivo],
      tipoCambio: [data?.tipoCambio],
      pagado: [data?.pagado],
      estadoSunat: [data?.estadoSunat],
      codigoTipoMoneda: [data?.codigoTipoMoneda],
      importeBruto: [data?.importeBruto?.toFixed(2)],
      valorIGV: [data?.valorIGV],
      igvTotal: [data?.igvTotal],
      iscTotal: [data?.iscTotal],
      otrTotal: [data?.otrTotal],
      otrcTotal: [data?.otrcTotal],
      importeTotal: [data?.importeTotal?.toFixed(2)],
      importeTotalLetra: [data?.importeTotalLetra],
      totalOpGravada: [data?.totalOpGravada?.toFixed(2)],
      totalOpInafecta: [data?.totalOpInafecta?.toFixed(2)],
      totalOpExonerada: [data?.totalOpExonerada?.toFixed(2)],
      totalOpGratuita: [data?.totalOpGratuita?.toFixed(2)],
      totalDescuento: [data?.totalDescuento?.toFixed(2)],
      ordenCompra: [data?.ordenCompra],
      guiaRemision: [data?.guiaRemision],
      codigoTipoNota: [data?.codigoTipoNota],
      codigoMotivoNota: [data?.codigoMotivoNota],
      estado: [data?.estado],

      // Extra
      tipoDocumentoIdentidadId: [data?.cliente?.tipoDocumentoIdentidadId],
      numeroDocumento: [data?.cliente?.numeroDocumento],
      nombreCliente: [data?.cliente?.nombre],
      cuentaCorriente: [data?.cuentaCorriente],
    });
  }

  // Inicio Angular
  ngOnInit() {
    this.onLoad();
    this.onLoadMaestras();
    this.selectedChangeTipoCaptacion(this.data.tipoCaptacionId);
  }

  // Obtener comprobante
  onLoad(){
    this.comprobantePagoService.getComprobantePagoById(this.data.comprobantePagoId).subscribe(
      response=>{
        if(response.success){
          this.dataSource.data = response.data.comprobantePagoDetalle;
          this.dataSource._updateChangeSubscription();
        }
      }
    );
  }

  // Maestras
  onLoadMaestras() {
    // Tipo Documento Identidad
    this.tipoDocumentoIdentidadService.getTipoDocumentoIdentidades().subscribe((response) => {
      if (response.success) {
        if (response.success) {
          let tipoDocsFactura = response.data;
          let tipoDocsBoleta = response.data;
          let tipoDocsEncargado = response.data;
          this.tipoDocIdentidadesEncargado = tipoDocsEncargado.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI || x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE);

          switch (this.data.tipoDocumentoId) {
            case this.tipoDocEnum.FACTURA:
              this.tipoDocIdentidades = tipoDocsFactura.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.RUC);
              break;
            case this.tipoDocEnum.BOLETA_VENTA:
              this.tipoDocIdentidades = tipoDocsBoleta.filter(x => x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.DNI || x.tipoDocumentoIdentidadId == this.tipoDocIdentidadEnum.CE);
              break;
            case this.tipoDocEnum.NOTA_CREDITO:
              this.tipoDocIdentidades = response.data;
              break;
            case this.tipoDocEnum.NOTA_DEBITO:
              this.tipoDocIdentidades = response.data;
              break;
            default:
              break;
          }
        }

      }
    });
    // Cuentas Corrientes
    this.cuentaCorrienteService.getCuentaCorrienteByEjecutora(this.data.unidadEjecutoraId).subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.numeroDenominacion = element.numero + " - " + element.denominacion;
        });
        this.cuentasCorrientes = response.data;
        this.filteredCuentasCorrientes = this.cuentasCorrientes.slice();
        let cuentas = this.cuentasCorrientes.filter(x => x.cuentaCorrienteId == this.data.cuentaCorrienteId)
        if (cuentas.length > 0) {
          this.form.patchValue({ cuentaCorriente: cuentas[0] });
        }
      }
    });

    // Tarifarios
    this.tarifarioService.getTarifarios().subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.codigoNombre = element.codigo + ' - ' + element.nombre;
        });
        this.tarifarios = response.data;
        this.filteredTarifarios = this.tarifarios.slice();
      }
    });

    // Catalogos
    this.catalogoBienService.getCatalogoBienes().subscribe((response) => {
      if (response.success) {
        response.data.forEach(element => {
          element.codigoDescripcion = element.codigo + " - " + element.descripcion;
        });
        this.catalogoBienes = response.data;
        this.filteredCatalogoBienes = this.catalogoBienes.slice();
        let cuentas = this.cuentasCorrientes.filter(x => x.cuentaCorrienteId == this.data.cuentaCorrienteId)
          if (cuentas.length > 0) {
            this.form.patchValue({ cuentaCorriente: cuentas[0] });
          }
      }
    });

    // Tipo Captaciones
    this.tipoCaptacionService.getTipoCaptaciones().subscribe((response) => {
      if (response.success) {
        this.tipoCaptaciones = response.data.filter(x => x.tipoCaptacionId != this.tipoCaptacionEnum.VARIOS);
      }
    });

    // tipo operaciones
    this.transversalService.getTipoOperaciones().subscribe((response) => {
      if (response.success) {
        this.tipoOperaciones = response.data;
      }
    });

    // tipo monedas
    this.transversalService.getTipoMonedas().subscribe((response) => {
      if (response.success) {
        this.tipoMonedas = response.data;
      }
    });

    // tipo Adquisicion
    this.transversalService.getTipoAdquisiciones().subscribe((response) => {
      if (response.success) {
        this.tipoAdquisiciones = response.data;
      }
    });

    // tipo IGV
    this.transversalService.getTipoIgvs().subscribe((response) => {
      if (response.success) {
        this.tipoIgvs = response.data;
      }
    });

    // tipo IGV
    this.transversalService.getTipoCondicionPagos().subscribe((response) => {
      if (response.success) {
        this.tipoCondicionPagos = response.data;
      }
    });

    // Tipo Fuentes
    this.tipoComprobantePagoService.getTipoComprobantePagos().subscribe((response) => {
      if (response.success) {
        this.tipoFuentes = response.data.filter(x => x.tipoComprobantePagoId == this.tipoComprobanteEnum.FACTURA || x.tipoComprobantePagoId == this.tipoComprobanteEnum.BOLETA_VENTA)
      }
    });

    // tipo Nota creditos
    this.transversalService.getTipoNotaCreditos().subscribe((response) => {
      if (response.success) {
        this.tipoNotaCreditos = response.data;
      }
    });

    // tipo Nota Débitos
    this.transversalService.getTipoNotaDebitos().subscribe((response) => {
      if (response.success) {
        this.tipoNotaDebitos = response.data;
      }
    });

    // Fuente origenes
    this.transversalService.getFuenteOrigenes().subscribe((response) => {
      if (response.success) {
        this.fuenteOrigenes = response.data;
      }
    });

  }

  // Select Tipo Captación
  selectedChangeTipoCaptacion(id) {
    switch (id) {
      case this.tipoCaptacionEnum.EFECTIVO:
        this.hiddenDeposito = true;
        this.hiddenCheque = true;
        break;
      case this.tipoCaptacionEnum.DEPOSITO_CUENTA_CORRRIENTE:
        this.hiddenDeposito = false;
        this.hiddenCheque = true;
        break;
      case this.tipoCaptacionEnum.CHEQUE:
        this.hiddenCheque = false;
        this.hiddenDeposito = true;
        break;
      default:
        break;
    }
  }

  // Guardar comprobante
  onSubmit() {
  }

  handleError(error) {
    this.messageService.msgError(MESSAGES.API.ERR_SERVICE, () => { });
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  onCloseDialog() {
    this.messageService.msgClose(MESSAGES.FORM.CLOSE_FORM, () => {
      this.onNoClick();
    });
  }

  onNoClick(): void {
    this.dialogRef.close(0);
  }

  formatMoney(i) {
    return Tools.formatMoney(i);
  }
    
}
