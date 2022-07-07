export class Tarifario {
    tarifarioId: number;
    clasificadorIngresoId: number;
    codigo: string;
    nombre: string;
    porcentajeUit: number;
    precio: number;
    grupoRecaudacionId: number;
    grupoRecaudacion: any;
    precioVariable: boolean;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    codigoNombre: string;
    index: number;
}

export class TarifarioFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    clasificadorIngresoId: number;
    codigo: string;
    nombre: string;
    estado: boolean;
}
