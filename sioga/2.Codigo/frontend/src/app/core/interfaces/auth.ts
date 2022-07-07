
export interface IUsuarioAuth {
    nombre: string;
    apellidoPaterno: string;
    apellidoMaterno: string;
    numeroDocumento: string;
    fechaNacimiento: any;
    correo: string;
    sesion: ISesionAuth;
    nombreCompleto: string;
    roles: IRolAuth[];
    menus: IMenuAuth[];
    modulos: IModuloAuth[];
}

export interface IMenuAuth {
    idMenu: number;
    codigo: string;
    nombreMenu: string;
    nombreIcono: string;
    ordenMenu: number;
    idMenuPadre: number;
    url: string;
    tipoOpcion: number;
    totalChildren: number;
}

export interface ISesionAuth {
    fechaCaducidad: any;
    fechaCreacion: any;
    fechaUltimaSesion: any;
}

export interface IRolAuth {
    idRol: number;
    codigo: string;
    nombreRol: string;
    porDefecto: boolean;
}

export interface IModuloAuth {
    idModulo: number;
    codigo: string;
    nombreModulo: string;
    nombreIcono: string;
    orden: number;
    url: string;
    tipoOpcion: number;
}
