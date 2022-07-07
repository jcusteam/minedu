using System;
using System.Collections.Generic;

namespace RecaudacionApiComprobantePago.Domain
{
    public class Documento
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

        public string NumeroComprobante { get; set; }
        public string FechaEmision { get; set; }
        public string HoraEmision { get; set; } // Fijo 12:00
        public string CodigoTipoComprobante { get; set; } // Codigo segun Equivalencia SUNAT
        public string CodigoTipoMoneda { get; set; }
        public string SerieNumeroCorrelativo { get; set; } // Serie + '-' + Numero Comprobante

        //Datos del Aquirente
        public string RUCAdquirente { get; set; } // RUC del Cliente
        public string TipoDocumentoAdquirente { get; set; } // Si es Boleta es DNI va 1 / RUC 6
        public string RazonSocialAdquirente { get; set; } // RAzon Social si es RUC o Nombre completo si es Boleta

        public string CondicionPagoEmisor { get; set; }  // Descripcion del tipo de pago p.e. EFECTIVO / TRANSFERENCIA BANCARIA
        public string DireccionAdquirente { get; set; }
        public Decimal ImporteBrutoComprobante { get; set; } // Monto total del Documento sin IGV
        public Decimal ValorIGV { get; set; } // Opcional 

        public string DigestValue { get; set; } // Para almacenar la Firma Digital del documento de respuesta de SUNAT

        public string SignatureValue { get; set; }// Para almacenar el HASH del documento de respuesta de SUNAT

        //Importes Totales
        public Decimal IGVTotalComprobante { get; set; } // Total del IGV de las operaciones gravadas
        public Decimal OTRCTotalComprobante { get; set; }
        public Decimal ImporteTotalComprobante { get; set; } // Monto Total de todo el documento

        public string MontoEnLetras { get; set; } // Del monto total

        public Decimal TotalVentaOpGravadas { get; set; }
        public Decimal TotalVentaOpInafectas { get; set; }
        public Decimal TotalVentaOpExoneradas { get; set; }
        public Decimal TotalVentaOpGratuitas { get; set; }
        public Decimal TotalDescuentos { get; set; }
        public string OrdenCompra { get; set; } // Opcional
        public string GuiaRemision { get; set; } // Opcional

        // CAmpos de Control
        public string sDocAdicional { get; set; } // Opcional
        public string sOrdenCompra { get; set; } // Opcional

        // Orden Compra
        public string nOrdenCompra { get; set; } // Opcional
        //Detalle Comprobante
        public List<DocumentoDetalle> DetalleDocumento { get; set; }
        //Detalle Documentos Adicionales
        public List<DocumentoAdicional> DetalleAdicionales { get; set; }
        public Decimal sFactorDescuento { get; set; } // Monto del factor de descuento - opcional solo para calculos
        // Nota de Credito / Debito
        public string CodigoTipoNotaCredito { get; set; } // Codigo del tipo de Nota de Credito segun equivalencia SUNAT
        public string CodigoMotivoNotaCredito { get; set; } // Codigo Motivo de la Nota de Credito segun la equivalencia de la SUNAT
        public string CodigoTipoNotaDebito { get; set; } // Codigo del tipo de Nota de Debito segun equivalencia SUNAT
        public string CodigoMotivoNotaDebito { get; set; }// Codigo Motivo de la Nota de Debito segun la equivalencia de la SUNAT
        public string MotivoSustentoDocumento { get; set; }  // Descripcion del motivo segun la equivalencia de la SUNAT
        public string SerieNumeroDocAfectado { get; set; } // Serie y Numero del documento Afectado p.e. F001-0001245
        public string CodigoTipoDocAfectado { get; set; } // Codigo de documento afectado segun equivalencia SUNAT p.e. Factura = 01
        public string FechaEmisionDocAfectado { get; set; } //
        public string sEstado { get; set; } // Almacena el estado devuelto por la respuesta de SUNAT


        public Documento()
        {
            DetalleDocumento = new List<DocumentoDetalle>();
            DetalleAdicionales = new List<DocumentoAdicional>();
        }

    }
}
