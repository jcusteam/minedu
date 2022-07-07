export class Uit {
    uitId: number;
    periodo: string;
    unidadMonetaria: string;
    valor: number;
    porcentaje: number;
    baseLegal: string;
    fechaRegistro: any;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class UitFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    periodo:string;
    estado:boolean;
}
