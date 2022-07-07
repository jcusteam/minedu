export class TipoComprobantePago {
    tipoComprobantePagoId: number;
    codigo: string;
    nombre: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class TipoComprobantePagoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    codigo: string;
    nombre: string;
    estado: boolean;
}
