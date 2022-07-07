export class UnidadMedida {
    unidadMedidaId: number;
    nombre: string;
    abreviatura: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class UnidadMedidaFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    nombre:string;
    estado:boolean;
}
