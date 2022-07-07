export const environment = {
  production: true,
  codigoSistema: "001351",
  codigoModulo: "01000000",
  secretKey: 'k4w4bun64m1N3dU',
  url: {
    urlPassport: 'https://test-webpassportv4seguridad-congelado.minedu.gob.pe:7777',
    urlSistema: 'https://testingsioga.minedu.gob.pe',
    apiGatewayDomain: 'agw-transversal-int-qa.minedu.gob.pe',
    apiGateway: 'https://agw-transversal-int-qa.minedu.gob.pe',
    apiEndPoints: {
      suiteKong: '/sioga-suite/api/v1',
      authorization: '/passport/api/v1/authorization',
    },
    apiPublic: 'https://api-sioga-public.minedu.gob.pe',
    apiPublicEndPoints: {
      suiteKongPublic: '',
      banco: '/api/public/bancos',
      clasificadorIngreso: '/api/public/clasificador-ingresos',
      cuentaCorriente: '/api/public/cuentas-corrientes',
      tipoReciboIngreso: '/api/public/tipos-recibos-ingresos',
      tipoDocumentoIdentidad: '/api/public/tipo-documento-identidad',
      registroLinea: '/api/public/registro-lineas',
    },
  }
};
