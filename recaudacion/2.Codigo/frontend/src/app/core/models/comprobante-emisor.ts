export class ComprobanteEmisor {
    comprobanteEmisorId: number;
    unidadEjecutoraId: number;
    firmante: string;
    numeroRuc: string;
    tipoDocumento: string;
    nombreComercial: string;
    razonSocial: string;
    ubigeo: string;
    direccion: string;
    urbanizacion: string;
    departamento: string;
    provincia: string;
    distrito: string;
    codigoPais: string;
    telefono: string;
    direccionAlternativa: string;
    numeroResolucion: string;
    usuarioOSE: string;
    claveOSE: string;
    correoEnvio: string;
    correoClave: string;
    serverMail: string;
    serverPort: string;
    nombreArchivoCer: string;
    nombreArchivoKey: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
}
export class ComprobanteEmisorEstado {
    comprobanteEmisorId: number;
    estado: boolean;
    usuarioModificador: string;
}

export class ComprobanteEmisorFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    estado: boolean;
}
