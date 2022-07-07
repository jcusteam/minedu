export class TipoDocumentoIdentidad {
    tipoDocumentoIdentidadId: number;
    codigo: string;
    nombre: string;
    descripcion: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class TipoDocumentoIdentidadFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    nombre: string;
    estado: boolean;
}
