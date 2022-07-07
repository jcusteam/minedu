// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  codigoSistema: "001491",
  secretKey: '$2a$12$F/IIygU5SlgGS9Nb1Vph6eB21ASGBgt0pjzAMJ1Z/NliH/v8bhJfS',
  siteKeyRecaptcha :'6LcAeLEeAAAAACp_r1C6gudsSi_jeAC1BGiMWFbb',
  url: {
    urlPassport: 'https://passport4seguridad-vpn1.minedu.gob.pe:1132',
    urlSistema: 'https://dev-sioga.minedu.gob.pe',
    apiGatewayDomain: 'siogaapigateway.minedu.gob.pe',
    apiGateway: 'https://siogaapigateway.minedu.gob.pe',
    apiEndPoints: {
      suiteKong:'',
      authorization: '/passport/api/v1/authorization',
    },
    apiPublic:'https://siogaapipublic.minedu.gob.pe',
    apiPublicEndPoints: {
      suiteKongPublic:'',
      banco: '/api/public/bancos',
      clasificadorIngreso: '/api/public/clasificador-ingresos',
      cuentaCorriente: '/api/public/cuentas-corrientes',
      tipoReciboIngreso: '/api/public/tipos-recibos-ingresos',
      tipoDocumentoIdentidad: '/api/public/tipo-documento-identidad',
      registroLinea: '/api/public/registro-lineas',
    },
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
