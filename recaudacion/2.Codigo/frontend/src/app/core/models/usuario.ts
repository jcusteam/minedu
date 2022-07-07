
export class Usuario {
  nombre: string;
  apellidoPaterno: string;
  apellidoMaterno: string;
  idTipoDocumento: number;
  nombreTipoDocumento: string;
  numeroDocumento: string;
  fechaNacimiento: string;
  correo: string;
  nombreCompleto: string;
  rol: Rol;
  roles: Rol[];
  menus: MenuAuth[];
  sesion: Sesion;
  roleRecaudacion:string;
}

export class Rol {
  idRol: number;
  codigo: string;
  nombreRol: string;
  idSede: number;
  nombreSede: string;
  idTipoSede: number;
  descriptionTipoSede: string;
  porDefecto: boolean
}

export class MenuAuth {
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


export class Sesion {
  fechaCaducidad: any;
  fechaCreacion: any;
  fechaUltimaSesion: any;
}


export class Accion {
  idPermiso: boolean;
  nombrePermiso: boolean;
}
