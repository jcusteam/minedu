export class CuentaCorriente {
    cuentaCorrienteId: number;
    bancoId: number;
    fuenteFinanciamientoId: number;
    unidadEjecutoraId: number;
    codigo: string;
    fecha: any;
    numero: string;
    denominacion: string;
    tipo: string;
    moneda: string;
    observacion: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    numeroDenominacion: string;
    index: number;
}

export class CuentaCorrienteFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    codigo: string;
    numero: string;
    denominacion: string;
    estado: boolean;
}

