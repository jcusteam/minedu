import { UnidadMedida } from "./unidadmedida";

export class CatalogoBien {
    catalogoBienId: number;
    clasificadorIngresoId: number;
    unidadMedidaId: number;
    unidadMedida: UnidadMedida;
    codigo: string;
    descripcion: string;
    stockMaximo: number;
    stockMinimo: number;
    puntoReorden: number;
    estado: boolean;
    saldo: number;
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
    codigoDescripcion: string;
}

export class CatalogoBienFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    clasificadorIngresoId: number;
    codigo: string;
    descripcion: string;
    estado: boolean;
}

export class CatalogoBienSaldo {
    catalogoBienId: number;
    clasificadorIngresoId: number;
    unidadMedidaId: number;
    unidadMedida: any;
    codigo: string;
    descripcion: string;
    estado: boolean;
    saldo: number;
    index: number;
}
