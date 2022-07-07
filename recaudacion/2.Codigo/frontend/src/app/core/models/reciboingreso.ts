import { ClasificadorIngreso } from "./clasificadoringreso";
import { Cliente } from "./cliente";
import { CuentaCorriente } from "./cuentacorriente";
import { DepositoBancoDetalle } from "./depositobanco";
import { FuenteFinanciamiento } from "./fuentefinanciamiento";
import { TipoReciboIngreso } from "./tiporeciboingreso";

export class ReciboIngreso {
    reciboIngresoId: number;
    unidadEjecutoraId: number;
    tipoReciboIngresoId: number;
    tipoReciboIngreso: TipoReciboIngreso;
    clienteId: number;
    cliente: Cliente;
    cuentaCorrienteId: number;
    cuentaCorriente: CuentaCorriente;
    fuenteFinanciamientoId: number;
    fuenteFinanciamiento: FuenteFinanciamiento;
    registroLineaId: number;
    tipoDocumentoId: number;
    numero: string;
    fechaEmision: any;
    tipoCaptacionId: number;
    depositoBancoDetalleId: number;
    depositoBancoDetalle: DepositoBancoDetalle;
    importeTotal: number;
    numeroDeposito: string;
    fechaDeposito: any;
    validarDeposito: string;
    numeroCheque: string;
    numeroOficio: string;
    numeroComprobantePago: string;
    expedienteSiaf: string;
    numeroResolucion: string;
    cartaOrden: string;
    liquidacionIngreso: string;
    papeletaDeposito: string;
    concepto: string;
    referencia: string;
    liquidacionId: number;
    estado: number;
    usuarioCreador: string;
    usuarioModificador: string;
    reciboIngresoDetalle: ReciboIngresoDetalle[];
    index: number;
}

export class ReciboIngresoEstado {
    reciboIngresoId: number;
    estado: number;
    usuarioModificador: string;
}
export class ReciboIngresoDetalle {
    reciboIngresoDetalleId: number;
    clasificadorIngresoId: number;
    clasificadorIngreso: ClasificadorIngreso;
    importe: number;
    referencia: string;
    estado: string;
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
}

export class ReciboIngresoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    tipoReciboIngresoId: number;
    clienteId: number;
    tipoDocumentoIdentidadId: number;
    numeroDocumento: string;
    clienteNombre: string;
    numero: string;
    tipoCaptacionId: number;
    estado: number;
    rol: string;
}

export class ReporteReciboIngreso {
    ejecutora: string;
    secEje: string;
    numeroRuc: string;
    numero: string;
    fecha: any;
    procedencia: string;
    glosa: string;
    cuentaCorriente: string;
    concepto: string;
    numeroComprobantePago: string;
    expedienteSiaf: string;
    numeroDeposito: string;
    numeroResolucion: string;
    parcial: number;
    total: number;
    pliego: string;
    fuenteFinanciamiento: string;
    unidad: string;
    codigo: string;
    codigoDos: string;
    cuentaDebe: string;
    cuentaHaber: string;
    boletaVenta: string;
    factura: string;
    numeroLiquidacion: string;
    liquidaciones: ReporteLiquidacionIngreso[];
    detalles: ReporteReciboIngresoDetalle[];
}

export class ReporteReciboIngresoDetalle {
    clasificador: string;
    parcial: number;
}

export class ReporteLiquidacionIngreso {
    clasificadorIngresoId: number;
    clasificador: string;
    codigo: string;
    nombre: string;
    nombreTipoCaptacion: string;
    total: number;
}