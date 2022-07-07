import { CatalogoBien } from './catalogobien';
import { Tarifario } from 'src/app/core/models/tarifario';
import { CuentaCorriente } from './cuentacorriente';
import { Cliente } from "./cliente";
import { IngresoPecosaDetalle } from "./ingresopecosa";

export class ComprobantePago {
    comprobantePagoId: number;
    clienteId: number;
    cliente: Cliente;
    unidadEjecutoraId: number;
    tipoComprobanteId: number;
    tipoDocumentoId: number;
    depositoBancoDetalleId: number;
    cuentaCorrienteId: number;
    cuentaCorriente: CuentaCorriente;
    tipoCaptacionId: number;
    comprobanteEmisorId: number;
    serie: string;
    correlativo: string;
    fechaEmision: any;
    fechaVencimiento: any;
    tipoAdquisicion: number;
    codigoTipoOperacion: string;
    tipoCondicionPago: number;
    numeroDeposito: string;
    fechaDeposito: any;
    validarDeposito: string;
    numeroCheque: string;
    encargadoTipoDocumento: number;
    encargadoNombre: string;
    encargadoNumeroDocumento: string;
    fuenteId: number;
    fuenteTipoDocumento: number;
    fuenteSerie: string;
    fuenteCorrelativo: string;
    fuenteOrigen: string;
    fuenteValidar: string;
    sustento: string;
    observacion: string;
    nombreArchivo: string;
    tipoCambio: number;
    pagado: true;
    estadoSunat: string;
    codigoTipoMoneda: string;
    importeBruto: number;
    valorIGV: number;
    igvTotal: number;
    iscTotal: number;
    otrTotal: number;
    otrcTotal: number;
    importeTotal: number;
    importeTotalLetra: string;
    totalOpGravada: number;
    totalOpInafecta: number;
    totalOpExonerada: number;
    totalOpGratuita: number;
    totalDescuento: number;
    ordenCompra: string;
    guiaRemision: string;
    codigoTipoNota: string;
    codigoMotivoNota: string;
    estado: number;
    usuarioCreador: string;
    usuarioModificador: string;
    comprobantePagoDetalle: ComprobantePagoDetalle[];
    index: number;
}

export class ComprobantePagoEstado {
    comprobantePagoId: number;
    estado: number;
    usuarioModificador: string;
}

export class ComprobantePagoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    tipoDocumentoId: number;
    clienteId: number;
    serie: string;
    correlativo: string;
    tipoCaptacionId: number;
    tipoAdquisicion: number;
    fechaInicio: any;
    fechaFin: any;
    rol: string;
}

export class ComprobantePagoFuente {
    unidadEjecutoraId: number;
    tipoComprobanteId: number;
    serie: string;
    correlativo: string;
}

export class ComprobantePagoDetalle {
    comprobantePagoDetalleId: number;
    comprobantePagoId: number;
    tipoAdquisicion: number;
    catalogoBienId: number;
    catalogoBien: CatalogoBien;
    tarifarioId: number;
    tarifario: Tarifario;
    clasificadorIngresoId: number;
    unidadMedida: string;
    cantidad: number;
    codigo: string;
    descripcion: string;
    codigoTipoMoneda: string;
    precioUnitario: number;
    precioSinIGV: number;
    totalItemSinIGV: number;
    codigoTipoPrecio: string;
    afectoIGV: boolean;
    igvItem: number;
    codigoTipoIGV: number;
    descuentoItem: number;
    descuentoTotal: number;
    factorDescuento: number;
    subTotal: number;
    valorVenta: number;
    ingresoPecosaDetalleId: number;
    ingresoPecosaDetalle: IngresoPecosaDetalle;
    serieFormato: string;
    serieDel: number;
    serieAl: number;
    estado: string;
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
}
