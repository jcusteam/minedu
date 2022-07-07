using System;
using System.Collections.Generic;

namespace RecaudacionApiComprobanteRetencion.Domain
{
    public class Retencion
    {

        //Datos del Emisor
        public string FirmanteEmisor { get; set; }
        public string NombreCertEmisor { get; set; } // Nombre de Archivo de certificado
        public string NombreKeyEmisior { get; set; } // Nombre de archivo de certificado key
        public string CorreoEnvioEmisor { get; set; } // Correo de envio
        public string CorreoKeyEmisor { get; set; } // Clave de correo de envio
        public string ServerMailEmisor { get; set; } // Servidor SMPT
        public string ServerPortEmisor { get; set; } // Puerto de Servidor SMTP
        public string RUCEmisor { get; set; }
        public string TipoDocumentoEmisor { get; set; }
        public string NombreComercialEmisor { get; set; }
        public string RazonSocialEmisor { get; set; }
        public string UbigeoEmisor { get; set; }
        public string DireccionEmisor { get; set; }
        public string UrbanizacionEmisor { get; set; }
        public string DepartamentoEmisor { get; set; }
        public string ProvinciaEmisor { get; set; }
        public string DistritoEmisor { get; set; }
        public string CodigoPaisEmisor { get; set; }
        public string TelefonoEmisor { get; set; } // Opcional
        public string DireccionAlternativaEmisor { get; set; }
        public string DireccionAlternativaSegEmisor { get; set; }
        public string NumeroResolucionEmisor { get; set; }
        public string UsuarioOseEmisor { get; set; }
        public string ClaveOseEmisor { get; set; }

        public string Observaciones { get; set; }
        public string CodigoTipoComprobante { get; set; }
        public string SerieComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public string FechaEmision { get; set; }



        //Datos del Aquirente
        public string RUCAdquirente { get; set; }
        public string TipoDocumentoAdquirente { get; set; }
        public string RazonSocialAdquirente { get; set; }
        public string UbigeoAdquirente { get; set; }
        public string DireccionAdquirente { get; set; }
        public string LocalidadAdquirente { get; set; }
        public string UrbanizacionAdquiriente { get; set; }
        public string DepartamentoAdquiriente { get; set; }
        public string ProvinciaAdquiriente { get; set; }
        public string DistritoAdquiriente { get; set; }
        public string CodigoPaisAdquiriente { get; set; }

        // Datos retencion
        public string CodigoTipoMoneda { get; set; }
        public string CodigoRetencion { get; set; }
        public Decimal PorcentajeRetencion { get; set; }
        public Decimal MontoTotalRetencion { get; set; }
        public Decimal MontoTotalPago { get; set; }

        /// <summary>
        /// Valor Resumen
        /// </summary>
        public string DigestValue { get; set; }

        /// <summary>
        /// Valor de la Firma Digital
        /// </summary>
        public string SignatureValue { get; set; }

        //Detalle Comprobante
        public List<DetalleRetencion> DetalleRetencion { get; set; }
        public Retencion()
        {
            DetalleRetencion = new List<DetalleRetencion>();
        }
    }
}
