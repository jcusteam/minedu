export class TipoReciboIngreso {
    tipoReciboIngresoId: number;
    nombre: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class TipoReciboIngresoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    nombre: string;
    estado: boolean;
}
