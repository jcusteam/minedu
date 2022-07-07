export class FuenteFinanciamiento {
    fuenteFinanciamientoId: number;
    codigo: string;
    descripcion: string;
    rubroCodigo: string;
    rubroDescripcion: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}

export class FuenteFinanciamientoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    fuenteFinanciamientoId: number;
    codigo: string;
    descripcion: string;
    estado: boolean;
}
