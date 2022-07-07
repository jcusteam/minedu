export class CuentaContable {
    cuentaContableId: number;
    codigo: string;
    descripcion: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}

export class CuentaContableFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    codigo: string;
    descripcion:string;
    estado: boolean;
}
