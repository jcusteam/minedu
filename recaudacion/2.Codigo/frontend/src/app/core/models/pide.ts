export class Reniec {
    numeroDni: string;
    apellidoPaterno: string;
    apellidoMaterno: string;
    nombres: string;
    paisNacimiento: string;
    departamentoNacimiento: string;
    provinciaNacimiento: string;
    distritoNacimiento: string;
    direccion: string;
    domicilioApp: string;
    nombreCompleto: string;
}

export class Migracion {
    strCalidadMigratoria: string;
    strNombres: string;
    strNumRespuesta: string;
    strPrimerApellido: string;
    strSegundoApellido: string;
    strNombreCompleto: string;
}


export class Sunat {
    cod_dep: string;
    cod_dist: string;
    cod_prov: string;
    ddp_ciiu: string;
    ddp_doble: string;
    ddp_estado: string;
    ddp_fecact: string;
    ddp_fecalt: string;
    ddp_fecbaj: string;
    ddp_flag22: string;
    ddp_identi: string;
    ddp_inter1: string;
    ddp_lllttt: string;
    ddp_mclase: string;
    ddp_nombre: string;
    ddp_nomvia: string;
    ddp_nomzon: string;
    ddp_numer1: string;
    ddp_numreg: string;
    ddp_numruc: string;
    ddp_reacti: string;
    ddp_refer1: string;
    ddp_secuen: string;
    ddp_tamano: string;
    ddp_tipvia: string;
    ddp_tipzon: string;
    ddp_tpoemp: string;
    ddp_ubigeo: string;
    ddp_userna: string;
    desc_ciiu: string;
    desc_dep: string;
    desc_dist: string;
    desc_estado: string;
    desc_flag22: string;
    desc_identi: string;
    desc_numreg: string;
    desc_prov: string;
    desc_tamano: string;
    desc_tipvia: string;
    desc_tipzon: string;
    desc_tpoemp: string;
    esActivo: boolean;
    esHabido: boolean;
    desc_domi_fiscal:string;
    representante: any;
}

export class SunatRepresentante {
    attributes: string;
    cod_cargo: string;
    cod_depar: any;
    desc_docide: string;
    num_ord_suce: string;
    rso_cargoo: string;
    rso_docide: string;
    rso_fecact: string;
    rso_fecnac: string;
    rso_nombre: string;
    rso_nrodoc: string;
    rso_numruc: string;
    rso_userna: any;
    rso_vdesde: string;
}