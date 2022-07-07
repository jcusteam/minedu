export class UnidadEjecutora {
    unidadEjecutoraId: number;
    nombre: string;
    secuencia: string;
    codigo: string;
    numeroRuc: string;
    direccion: string;
    correo: string;
    telefono: string;
    celular: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class UnidadEjecutoraFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    codigo:string;
    nombre:string;
    estado:boolean;
}
