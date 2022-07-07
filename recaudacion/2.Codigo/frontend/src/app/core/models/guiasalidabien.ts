export class GuiaSalidaBien {
    guiaSalidaBienId: number;
    unidadEjecutoraId: number;
    tipoDocumentoId:number;
    numero: string;
    fechaRegistro: any;
    justificacion: string;
    estado: number;
    estadoNombre: string;
    usuarioCreador: string;
    usuarioModificador: string;
    guiaSalidaBienDetalle: any;
    index:number;
}

export class GuiaSalidaBienEstado {
    guiaSalidaBienId: number;
    estado: number;
    usuarioModificador: string;
}

export class GuiaSalidaBienFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    unidadEjecutoraId: number;
    numero: number;
    fechaInicio: any;
    fechaFin: any;
    estado: number;
    rol: string;
}

export class GuiaSalidaBienDetalle {
    guiaSalidaBienDetalleId: number;
    guiaSalidaBienId: number;
    catalogoBienId: number;
    catalogoBien: any;
    ingresoPecosaDetalleId: number;
    ingresoPecosaDetalle:any;
    cantidad: number;
    serieFormato: string;
    serieDel: number;
    serieAl: number;
    estado:string;
    anioPecosa: number;
    numeroPecosa: number;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
