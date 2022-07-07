export class IngresoPecosa {
    ingresoPecosaId: number;
    unidadEjecutoraId: number;
    tipoDocumentoId: number;
    anioPecosa: number;
    tipoBien: string;
    numeroPecosa: number;
    fechaPecosa: any;
    nombreAlmacen: string;
    motivoPedido: string;
    fechaRegistro: any;
    estado: number;
    estadoNombre: string;
    usuarioCreador: string;
    usuarioModificador: string;
    ingresoPecosaDetalle: IngresoPecosaDetalle[];
    index:number;
}

export class IngresoPecosaEstado {
    ingresoPecosaId: number;
    estado: number;
    usuarioModificador: string;
}

export class IngresoPecosaFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn:string;
    sortOrder: string;
    unidadEjecutoraId: number;
    anioPecosa: number;
    tipoBien: string;
    numeroPecosa: number;
    fechaInicio: any;
    fechaFin: any;
    estado: number;
    rol: string;
}


export class IngresoPecosaDetalle {
    ingresoPecosaDetalleId: number;
    ingresoPecosaId: number;
    anioPecosa: number;//ops
    numeroPecosa: number; //ops
    catalogoBienId: number;
    unidadMedida: string;
    codigoItem: string;
    nombreItem: string;
    nombreMarca: string;
    cantidad: number;
    cantidadSalida: number;
    saldo: number;//ops
    precioUnitario: number;
    valorTotal: number;
    serieFormato: string;
    serieDel: number;
    serieAl: number;
    serieDelSalida: number;//ops
    serieAlSalida: number;//ops
    estado: string;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}


