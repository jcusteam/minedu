export class TipoDocumento {
  tipoDocumentoId: number;
  nombre: string;
  abreviatura: string;
  estado: boolean;
  usuarioCreador: string;
  usuarioModificador: string;
  index:number;
}

export class TipoDocumentoEstado {
  tipoDocumentoId: number;
  estado: boolean;
  usuarioModificador: string;
}
export class TipoDocumentoFilter {
  pageNumber: number;
  pageSize: number;
  sortColumn: string;
  sortOrder: string;
  nombre: string;
  estado: boolean;
}
