import { ClasificadorIngreso } from "./clasificadoringreso";
import { FuenteFinanciamiento } from "./fuentefinanciamiento";
import { TipoCaptacion } from "./tipocaptacion";

export class Liquidacion {
    liquidacionId: number;
    unidadEjecutoraId: number;
    tipoDocumentoId: number;
    fuenteFinanciamientoId: number;
    fuenteFinanciamiento: FuenteFinanciamiento;
    clienteId: number;
    reciboIngresoId: number;
    cliente: any;
    cuentaCorrienteId: number;
    cuentaCorriente: any;
    numero: string;
    procedencia: string;
    fechaRegistro: any;
    total: number;
    factura: string;
    boletaVenta: string;
    estado: number;
    estadoNombre: string;
    liquidacionDetalle: LiquidacionDetalle[];
    usuarioCreador: string;
    usuarioModificador: string;
    index: number;
}
export class LiquidacionEstado {
    liquidacionId: number;
    estado: number;
    usuarioModificador: string;
}
export class LiquidacionDetalle {
    clasificadorIngresoId: number;
    clasificadorIngreso: ClasificadorIngreso;
    tipoCaptacionId: number;
    tipoCaptacion: TipoCaptacion;
    importeParcial: number;
    usuarioCreador: string;
    usuarioModificador: string;
    reciboIngresoDetalle: any;
    index: number;
}

export class LiquidacionFilter {
    pageNumber: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: string;
    unidadEjecutoraId: number;
    numero: string;
    estado: number;
    rol: string;
}
