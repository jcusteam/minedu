import { Cliente } from "./cliente";
import { CuentaCorriente } from "./cuentacorriente";

export class DepositoBanco {
    depositoBancoId: number;
    unidadEjecutoraId: number;
    bancoId: number;
    cuentaCorrienteId: number;
    cuentaCorriente: CuentaCorriente;
    tipoDocumentoId: number;
    numero: string;
    importe: number;
    fechaDeposito: any;
    fechaRegistro: any;
    nombreArchivo: string;
    cantidad: number;
    estado: number;
    estadoNombre: string;
    usuarioCreador: string;
    usuarioModificador: string;
    depositoBancoDetalle: DepositoBancoDetalle[];
    index: number;
}

export class DepositoBancoEstado {
    depositoBancoId: number;
    estado: number;
    usuarioModificador: string;
}

export class DepositoBancoFile {
    cantidad: number;
    importeTotal: number;
    nombreArchivo: string;
    detalles: DepositoBancoDetalle[];
}

export class DepositoBancoFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    bancoId: number;
    cuentaCorrienteId: number;
    numero: string;
    nombreArchivo: string;
    utilizado: boolean;
    estado: number;
    rol: string;
}

export class DepositoBancoDetalle {
    depositoBancoDetalleId: number;
    depositoBancoId: number;
    clienteId: number;
    cliente: Cliente;
    numeroDeposito: string;
    importe: number;
    fechaDeposito: any;
    secuencia: string;
    tipoDocumento: string;
    tipoDocumentoNombre: string;
    serieDocumento: string;
    numeroDocumento: string;
    fechaDocumento: any;
    utilizado: boolean;
    estado: string;
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
}
