export class TipoCaptacion {
    tipoCaptacionId: number;
    nombre: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}

export class TipoCaptacionFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    nombre: string;
    estado: boolean;
}
