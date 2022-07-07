export class Banco {
    bancoId: number;
    codigo: string;
    nombre: string;
    estado: boolean;
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
}

export class BancoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    nombre: string;
    estado: boolean;
}
