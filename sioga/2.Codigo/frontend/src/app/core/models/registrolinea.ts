import { Cliente } from 'src/app/core/models/cliente';
export class RegistroLinea {
  registroLineaId: number;
  unidadEjecutoraId: number;
  cuentaCorrienteId: number;
  bancoId: number;
  clienteId: number;
  cliente:Cliente;
  tipoDocumentoId: number;
  numero: string;
  fechaRegistro:any;
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
}

export class RegistroLineaDetalle {
  registroLineaDetalleId: number;
  registroLineaId: number;
  clasificadorIngresoId: number;
  clasificadorIngreso: any;
  importe: number;
  referencia: string;
  estado:string;
  usuarioCreador: string;
  usuarioModificador: string;
}
