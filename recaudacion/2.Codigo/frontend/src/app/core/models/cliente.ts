export class Cliente {
    clienteId: number;
    tipoDocumentoIdentidadId: number;
    tipoDocumentoIdentidad:any;
    TipoDocumentoNombre:string;
    numeroDocumento: string;
    nombre: string;
    direccion: string;
    correo: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}

export class ClienteFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    tipoDocumentoIdentidadId: number;
    numeroDocumento: string;
    nombre:string;
    estado: boolean;
}

