export class GrupoRecaudacion {
    grupoRecaudacionId: number;
    nombre: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index:number;
}
export class GrupoRecaudacionFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    nombre: string;
    estado: boolean;
}
