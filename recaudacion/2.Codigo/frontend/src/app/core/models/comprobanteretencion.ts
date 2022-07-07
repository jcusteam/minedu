import { Cliente } from './cliente';
export class ComprobanteRetencion {
    comprobanteRetencionId: number;
    clienteId: number;
    cliente: Cliente;
    unidadEjecutoraId: number;
    tipoDocumentoId: number;
    tipoComprobanteId: number;
    serie: string;
    correlativo: string;
    fechaEmision: any;
    periodo: any;
    regimenRetencion: string;
    total: number;
    totalPago: number;
    porcentaje: number;
    nombreArchivo: string;
    observacion: string;
    estadoSunat: string;
    estado: number;
    usuarioCreador: string;
    usuarioModificador: string;
    comprobanteRetencionDetalle: ComprobanteRetencionDetalle[];
    regimenRetencionDesc: string;
    index: number;

}

export class ComprobanteRetencionEstado {
    comprobanteRetencionId: number;
    estado: number;
    usuarioModificador: string;
}

export class ComprobanteRetencionFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    clienteId: number;
    serie: string;
    correlativo: string;
    fechaInicio: any;
    fechaFin: any;
    rol: string;
}

export class ComprobanteRetencionDetalle {
    comprobanteRetencionDetalleId: number;
    comprobanteRetencionId: number;
    comprobantePagoId: number;
    tipoDocumento: string;
    tipoDocumentoNombre: string;
    serie: string;
    correlativo: string;
    fechaEmision: any;
    importeTotal: number;
    tipoModena: string;
    importeOperacion: number;
    modificaNotaCredito: boolean;
    fechaPago: any;
    tipoCambio: number;
    numeroCorrelativoPago: number;
    importePago: number;
    tasa: number;
    importeRetenido: number;
    fechaRetencion: any;
    importeNetoPagado: number;
    estado: string;
    usuarioCreador: string;
    usuarioModificador: string;
    regimenRetencionDesc: string;
    index: number;
}
