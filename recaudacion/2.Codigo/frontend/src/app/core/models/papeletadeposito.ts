import { CuentaCorriente } from "./cuentacorriente";
import { ReciboIngreso } from "./reciboingreso";

export class PapeletaDeposito {
  papeletaDepositoId: number;
  unidadEjecutoraId: number;
  bancoId: number;
  cuentaCorrienteId: number;
  cuentaCorriente: CuentaCorriente;
  tipoDocumentoId: number;
  numero: string;
  fecha: any;
  monto: number;
  descripcion: string;
  estado: number;
  usuarioCreador: string;
  usuarioModificador: string;
  papeletaDepositoDetalle: PapeletaDepositoDetalle[];
  index: number;
}

export class PapeletaDepositoEstado {
  papeletaDepositoId: number;
  estado: number;
  usuarioModificador: string;
}

export class PapeletaDepositoFilter {
  pageNumber: number;
  pageSize: number;
  sortColumn: string;
  sortOrder: string;
  unidadEjecutoraId: number;
  bancoId: number;
  cuentaCorrienteId: number;
  numero: string;
  estado: number;
  rol: string;
}

export class PapeletaDepositoDetalle {
  papeletaDepositoDetalleId: number;
  papeletaDepositoId: number;
  reciboIngresoId: number;
  reciboIngreso: ReciboIngreso;
  monto: number;
  estado: string;
  usuarioCreador: string;
  usuarioModificador: string;
  index: number;
}
