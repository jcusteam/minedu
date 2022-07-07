export class ClasificadorIngreso {
    clasificadorIngresoId: number;
    codigo: string;
    descripcion: string;
    cuentaContableIdDebe: number;
    cuentaContableIdHaber: number;
    tipoTransaccion: number;
    generica: number;
    subGenerica: number;
    subGenericaDetalle: number;
    especifica: number;
    especificaDetalle: number;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}

export class ClasificadorIngresoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    codigo:string;
    descripcion:string;
    estado:boolean;
}
