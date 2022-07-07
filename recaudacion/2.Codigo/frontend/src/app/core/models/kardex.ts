export class Kardex {
    kardexId:number;
    documento: string;
    anioPecosa: number;
    numeroPecosa: number;
    fecha: any;
    entradaDocumento: string;
    entradaDel: number;
    entradaAl: number;
    entradaTotal: number;
    salidaDocumento: string;
    salidaDocumentoNumero: string;
    salidaDel: string;
    salidaAl: string;
    salidaTotal: number;
    saldo: number;
    index: number;
}

export class KardexSaldo {
    codigo: string;
    descripcion: string;
    kardexs: Kardex[];
}
