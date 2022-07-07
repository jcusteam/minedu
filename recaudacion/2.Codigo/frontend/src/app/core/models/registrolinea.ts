import { Cliente } from './cliente';
import { CuentaCorriente } from './cuentacorriente';
import { Banco } from './banco';

export class RegistroLinea {
  registroLineaId: number;
  unidadEjecutoraId: number;
  cuentaCorrienteId: number;
  cuentaCorriente:CuentaCorriente;
  bancoId: number;
  banco:Banco;
  clienteId: number;
  cliente:Cliente;
  tipoDocumentoId: number;
  numero: string;
  fechaRegistro: any;
  tipoReciboIngresoId: number;
  depositoBancoDetalleId: number;
  numeroDeposito: string;
  importeDeposito: number;
  fechaDeposito: any;
  validarDeposito: string;
  numeroOficio: string;
  numeroComprobantePago: string;
  expedienteSiaf: string;
  numeroResolucion: string;
  expedienteESinad: string;
  numeroESinad:number;
  observacion: string;
  estado: number;
  usuarioCreador: string;
  usuarioModificador: string;
  registroLineaDetalle: RegistroLineaDetalle[]
  index: number;
}
export class RegistroLineaEstado {
  registroLineaId: number;
  estado: number;
  observacion: string;
  usuarioModificador: string;
}

export class RegistroLineaFilter {
  pageNumber: number;
  pageSize: number;
  sortColumn: string;
  sortOrder: string;
  unidadEjecutoraId: number;
  cuentaCorrienteId: number;
  bancoId: number;
  clienteId: number;
  numero: string;
  tipoDocumentoIdentidadId: number;
  numeroDocumento: string;
  clienteNombre: string;
  tipoReciboIngresoId: number;
  estado: number;
  rol: string;
}

export class RegistroLineaDetalle {
  registroLineaDetalleId: number;
  registroLineaId: number;
  clasificadorIngresoId: number;
  clasificadorIngreso: any;
  importe: number;
  referencia: string;
  usuarioCreador: string;
  usuarioModificador: string;
  index: number;
}
