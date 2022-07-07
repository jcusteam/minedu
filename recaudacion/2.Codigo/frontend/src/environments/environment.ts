// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  codigoSistema: "001491",
  codigoModulo: "01000000",
  secretKey: '$2a$12$F/IIygU5SlgGS9Nb1Vph6eB21ASGBgt0pjzAMJ1Z/NliH/v8bhJfS',
  url: {
    urlSistema: 'https://dev-sioga.minedu.gob.pe',
    apiGatewayDomain: 'siogaapigateway.minedu.gob.pe',
    apiGateway: 'https://siogaapigateway.minedu.gob.pe',
    apiEndPoints: {
      suiteKong:'',
      authorization: '/passport/api/v1/authorization',
      banco: '/comunes/banco/api/v1/bancos',
      catalogoBienSaldo: '/recaudacion/ingresopecosa/api/v1/ingreso-pecosa',
      catalogoBien: '/comunes/catalogobien/api/v1/catalogo-bienes',
      clasificadorIngreso: '/comunes/clasificadoringreso/api/v1/clasificador-ingresos',
      cliente: '/comunes/cliente/api/v1/clientes',
      comprobanteEmisor: '/recaudacion/comprobanteemisor/api/v1/comprobantes-emisor',
      comprobantePago: '/recaudacion/comprobantepago/api/v1/comprobante-pagos',
      comprobanteRetencion: '/recaudacion/comprobanteretencion/api/v1/comprobante-retenciones',
      cuentaContable: '/comunes/cuentacontable/api/v1/cuentas-contables',
      cuentaCorriente: '/comunes/cuentacorriente/api/v1/cuentas-corrientes',
      depositoBanco: '/recaudacion/depositobanco/api/v1/deposito-bancos',
      estado: '/recaudacion/estado/api/v1/estados',
      fileServer: '/recaudacion/fileserver/api/v1/file-server',
      fuenteFinanciamiento: '/comunes/fuentefinanciamiento/api/v1/fuentes-financiamiento',
      grupoRecaudacion: '/comunes/gruporecaudacion/api/v1/grupos-recaudacion',
      guiaSalida: '/recaudacion/guiasalidabien/api/v1/guia-salida-bienes',
      ingresoPecosa: '/recaudacion/ingresopecosa/api/v1/ingreso-pecosa',
      kardex: '/recaudacion/kardex/api/v1/kardex',
      liquidacion: '/recaudacion/liquidacion/api/v1/liquidaciones',
      papeletaDeposito: '/recaudacion/papeletadeposito/api/v1/papeleta-depositos',
      parametro: '/recaudacion/parametro/api/v1/parametros',
      pedidoPecosa: '/recaudacion/pedidopecosa/api/v1/pedido-pecosa',
      pide: '/pide/api/v1/pide',
      reciboIngreso: '/recaudacion/reciboingreso/api/v1/recibo-ingresos',
      registroLinea: '/recaudacion/registrolinea/api/v1/registro-lineas',
      reporte: '/recaudacion/reporte/api/v1/reportes',
      tarifario: '/comunes/tarifario/api/v1/tarifario',
      tipoCaptacion: '/comunes/tipocaptacion/api/v1/tipo-captaciones',
      tipoComprobantePago: '/comunes/tipocomprobantepago/api/v1/tipos-comprobantes-pago',
      tipoDocumentoIdentidad: '/comunes/tipodocumentoidentidad/api/v1/tipo-documento-identidad',
      tipoDocumento: '/recaudacion/tipodocumento/api/v1/tipo-documentos',
      tipoReciboIngreso: '/comunes/tiporeciboingreso/api/v1/tipos-recibos-ingresos',
      uit: '/comunes/uit/api/v1/uit',
      unidadEjecutora: '/comunes/unidadejecutora/api/v1/unidades-ejecutoras',
      unidadMedida: '/comunes/unidadmedida/api/v1/unidades-medida',
    }
  }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
