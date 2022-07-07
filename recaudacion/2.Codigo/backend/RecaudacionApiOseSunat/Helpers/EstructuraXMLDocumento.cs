using System;
using System.Text;
using RecaudacionApiOseSunat.Domain;
using RecaudacionUtils;

namespace RecaudacionApiOseSunat.Helpers
{
    public class EstructuraXMLDocumento
    {
        public StringBuilder ArmarXmlFacturaUBL21(Documento factura, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding =\"UTF-8\"?>");

            xmlCab.Append("<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            /* INI TAG <ext:UBLExtensions> */
            xmlCab.Append("<ext:UBLExtensions><ext:UBLExtension><ext:ExtensionContent/></ext:UBLExtension></ext:UBLExtensions>");
            /* INI TAG <ext:UBLExtension> */
            //xmlCab.Append("<ext:UBLExtension><ext:UBLExtensions>;
            /* INI TAG <ext:ExtensionContent> */
            //xmlCab.Append("<ext:ExtensionContent>");
            ///* INI TAG <ds:Signature Id="SignatureSP"> */
            //xmlCab.Append("<ds:Signature Id=\"SignatureSP\">");
            //xmlCab.Append("</ds:Signature>");
            /* FIN TAG <ds:Signature Id="SignatureSP"> */
            //xmlCab.Append("<ext:ExtensionContent/>");
            /* FIN TAG <ext:ExtensionContent> */
            //xmlCab.Append("</ext:UBLExtension>");
            /* FIN TAG <ext:UBLExtension> */
            //xmlCab.Append("</ext:UBLExtensions>");
            /* Version XML*/
            xmlCab.Append("<cbc:UBLVersionID>2.1</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>2.0</cbc:CustomizationID>");
            /* FIN Version XML */
            /* INI Numero Documento */
            xmlCab.Append("<cbc:ID>" + factura.NumeroComprobante + "</cbc:ID>");
            /* Fin Numero Documento */
            /* INI Fecha y Hora del Documento */
            xmlCab.Append("<cbc:IssueDate>" + factura.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cbc:IssueTime>" + factura.HoraEmision + "</cbc:IssueTime>");
            /* FIN Fecha y Hora del Documento */
            /* INI Tipo Documento */
            xmlCab.Append("<cbc:InvoiceTypeCode listID=\"0101\""); // Tipo de Operacion Catalogo 51 - 0101 Venta Interna
            xmlCab.Append(" listAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append(" listName=\"Tipo de Documento\" ");
            xmlCab.Append(" listURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01\">" + factura.CodigoTipoComprobante + "</cbc:InvoiceTypeCode>");
            /* Fin Tipo Documento */
            /* INI monto en Letras */
            /* xmlCab.Append(" <cbc:Note languageLocaleID=\"1000\"><![CDATA["+ new Funciones().LimpiarCaracteresInvalidos(factura.MontoEnLetras) + "]]></cbc:Note>"); */
            xmlCab.Append(" <cbc:Note><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.MontoEnLetras) + "]]></cbc:Note>");
            /* FIN monto en Letras */
            /* INI Moneda */
            xmlCab.Append(" <cbc:DocumentCurrencyCode listID=\"ISO 4217 Alpha\" listName=\"Currency\" listAgencyName=\"United Nations Economic Commission for Europe\">");
            xmlCab.Append(factura.CodigoTipoMoneda + "</cbc:DocumentCurrencyCode>");
            /* FIN Moneda */
            /* INI Cantidad de Items de la Factura */
            xmlCab.Append("<cbc:LineCountNumeric>" + factura.DetalleDocumento.Count.ToString() + "</cbc:LineCountNumeric>");
            /* FIN Cantidad de Items de la Factura */
            /* INI Orden de compra Cliente */
            if (factura.sOrdenCompra == "S")
            {
                xmlCab.Append("<cac:OrderReference><cbc:ID>" + factura.nOrdenCompra + "</cbc:ID></cac:OrderReference>");
            }
            /* FIN Orden de compra Cliente */

            /* INI Documentos Adicionales (NO ANTICIPO)*/
            if (factura.sDocAdicional == "S")
            {
                foreach (DocumentoAdicional da in factura.DetalleAdicionales)
                {
                    xmlCab.Append("<cac:AdditionalDocumentReference>");
                    xmlCab.Append("<cbc:ID>" + da.NumeroDocumento + "</cbc:ID>");
                    xmlCab.Append("<cbc:DocumentTypeCode>" + da.TipoDocumento + "</cbc:DocumentTypeCode>");
                    xmlCab.Append("</cac:AdditionalDocumentReference>");
                }
            }
            /* FIN Documentos Adicionales (NO ANTICIPO)*/
            /* INI TAG Signature */
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append("<cbc:ID>IDSignKG</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + factura.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            /* FIN TAG Signature */

            /* INI TAG "<cac:AccountingSupplierParty>" */
            xmlCab.Append("<cac:AccountingSupplierParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"" + factura.TipoDocumentoEmisor + "\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + factura.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyName> -- Nombre Comercial */
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            /* FIN TAG <cac:PartyName> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Emisor*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Emisor */
            xmlCab.Append("<cac:RegistrationAddress>");
            /*xmlCab.Append("<cbc:ID>" + factura.UbigeoEmisor + "</cbc:ID>");*/
            xmlCab.Append("<cbc:AddressTypeCode>0000</cbc:AddressTypeCode>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + factura.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + factura.ProvinciaEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + factura.DepartamentoEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + factura.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.DireccionEmisor) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + factura.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity>*/
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingSupplierParty>");
            /* FIN TAG <cac:AccountingSupplierParty> */
            /* INI TAG <cac:AccountingCustomerParty> -- Datos Adquiriente " */
            xmlCab.Append("<cac:AccountingCustomerParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"" + factura.TipoDocumentoAdquirente + "\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + factura.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Adquiriente*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Adquiriente*/
            xmlCab.Append("<cac:RegistrationAddress>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(factura.DireccionAdquirente) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>PE</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity> */
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingCustomerParty>");
            /* FIN TAG <cac:AccountingCustomerParty> */
            if (factura.TotalDescuentos > 0)
            {
                /* INI TAG <cac:AllowanceCharge> */
                xmlCab.Append("<cac:AllowanceCharge>");
                xmlCab.Append("<cbc:ChargeIndicator>false</cbc:ChargeIndicator>");
                xmlCab.Append("<cbc:AllowanceChargeReasonCode>00</cbc:AllowanceChargeReasonCode>");
                xmlCab.Append("<cbc:MultiplierFactorNumeric>" + factura.sFactorDescuento.ToString("#0.00") + "</cbc:MultiplierFactorNumeric>");
                xmlCab.Append("<cbc:Amount currencyID=\"PEN\">" + factura.TotalDescuentos.ToString("#0.00") + "</cbc:Amount>");
                xmlCab.Append("<cbc:BaseAmount currencyID=\"PEN\">" + factura.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:BaseAmount>");
                xmlCab.Append("</cac:AllowanceCharge>");
                /* FIN TAG <cac:AllowanceCharge> */
            }

            /* INI TAG <cac:PaymentTerms> */
            xmlCab.Append("<cac:PaymentTerms><cbc:ID>FormaPago</cbc:ID><cbc:PaymentMeansID>Contado</cbc:PaymentMeansID></cac:PaymentTerms>");
            /* FIN TAG <cac:PaymentTerms> */

            /* INI TAG <cac:TaxTotal> */
            xmlCab.Append("<cac:TaxTotal>");
            xmlCab.Append("<cbc:TaxAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
            /* OPeraciones Gravadas */
            if (factura.TotalVentaOpGravadas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            if (factura.TotalVentaOpGratuitas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.TotalVentaOpGratuitas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }

            #region Exonerados

            if (factura.TotalVentaOpExoneradas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.TotalVentaOpExoneradas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion

            #region INAFECTAS

            if (factura.TotalVentaOpInafectas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.TotalVentaOpInafectas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID schemeAgencyName=\"PE:SUNAT\" schemeID=\"UN/ECE 5153\" schemeName=\"Codigo de tributos\" >9998</cbc:ID>");
                xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion
            xmlCab.Append("</cac:TaxTotal>");
            /* FIN TAG <cac:TaxTotal> */
            /* INI TAG <cac:LegalMonetaryTotal> -- Totales */
            xmlCab.Append("<cac:LegalMonetaryTotal>");
            //if (factura.TotalVentaOpGravadas > 0)
            //{
            //30
            xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteBrutoComprobante.ToString("#0.00") + "</cbc:LineExtensionAmount>");
            //31
            xmlCab.Append((factura.ImporteTotalComprobante > 0 ? "<cbc:TaxInclusiveAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxInclusiveAmount>" : "<cbc:TaxInclusiveAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + 0.00.ToString("#0.00") + "</cbc:TaxInclusiveAmount>"));
            /*xmlCab.Append("<cbc:TaxExclusiveAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxExclusiveAmount>");*/
            //32
            xmlCab.Append((factura.TotalDescuentos > 0 ? "<cbc:AllowanceTotalAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.TotalDescuentos.ToString("#0.00") + "</cbc:AllowanceTotalAmount>" : String.Empty));
            //33
            xmlCab.Append((factura.OTRCTotalComprobante > 0 ? "<cbc:ChargeTotalAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.OTRCTotalComprobante.ToString("#0.00") + "</cbc:ChargeTotalAmount>" : String.Empty));
            //34 = 31-32+33 
            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + ((factura.ImporteTotalComprobante > 0 ? factura.ImporteTotalComprobante : factura.ImporteBrutoComprobante) - factura.TotalDescuentos + factura.OTRCTotalComprobante).ToString("#0.00") + "</cbc:PayableAmount>");
            //xmlCab.Append("<cbc:PayableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PayableAmount>");
            xmlCab.Append("</cac:LegalMonetaryTotal>");
            //}

            /* FIN TAG <cac:LegalMonetaryTotal>*/
            /* INI Detalle de Factura */
            foreach (DocumentoDetalle item in factura.DetalleDocumento)
            {
                /* INI TAG <cac:InvoiceLine>  -- Detalle de Item de la factura */
                xmlCab.Append("<cac:InvoiceLine>");
                xmlCab.Append("<cbc:ID>" + item.NumeroItem + "</cbc:ID>");
                xmlCab.Append("<cbc:InvoicedQuantity unitCode=\"" + item.UnidadMedida + "\">" + item.Cantidad.ToString("#0") + "</cbc:InvoicedQuantity>");
                /* Total del Item sin IGV */
                xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.TotalItemSinIGV.ToString("#0.00") + "</cbc:LineExtensionAmount>");
                /* INI TAG <cac:PricingReference>*/
                xmlCab.Append("<cac:PricingReference>");
                /* INI TAG <cac:AlternativeConditionPrice> - */
                xmlCab.Append("<cac:AlternativeConditionPrice>");
                /* Precio Unitario incluyendo IGV */
                // xmlCab.Append("<cbc:PriceAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + item.ValorVentaxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + item.ValorUnitarioxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceTypeCode>" + item.CodigoTipoPrecio + "</cbc:PriceTypeCode>");
                xmlCab.Append("</cac:AlternativeConditionPrice>");
                /* FIN TAG <cac:AlternativeConditionPrice> */
                xmlCab.Append("</cac:PricingReference>");
                /* FIN TAG <cac:PricingReference> */
                /* INI TAG <cac:AllowanceCharge> */
                if (item.DescuentoxItem > 0)
                {
                    xmlCab.Append("<cac:AllowanceCharge>");
                    xmlCab.Append("<cbc:ChargeIndicator>false</cbc:ChargeIndicator>");
                    xmlCab.Append("<cbc:AllowanceChargeReasonCode>00</cbc:AllowanceChargeReasonCode>");
                    xmlCab.Append("<cbc:MultiplierFactorNumeric>" + item.sFactorDescuento.ToString("#0.0000") + "</cbc:MultiplierFactorNumeric>");
                    xmlCab.Append("<cbc:Amount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.DescuentoxItem.ToString("#0.00") + "</cbc:Amount>");
                    xmlCab.Append("<cbc:BaseAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.DescuentoTotal.ToString("#0.00") + "</cbc:BaseAmount>");
                    xmlCab.Append("</cac:AllowanceCharge>");
                }
                #region CargoItem
                if (item.CargoxItem > 0)
                {
                    xmlCab.Append("<cac:AllowanceCharge>");
                    xmlCab.Append("<cbc:ChargeIndicator>true</cbc:ChargeIndicator>");
                    xmlCab.Append("<cbc:AllowanceChargeReasonCode>50</cbc:AllowanceChargeReasonCode>");
                    xmlCab.Append("<cbc:MultiplierFactorNumeric>" + item.sFactorCargo.ToString("#0.0000") + "</cbc:MultiplierFactorNumeric>");
                    xmlCab.Append("<cbc:Amount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.CargoxItem.ToString("#0.00") + "</cbc:Amount>");
                    xmlCab.Append("<cbc:BaseAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.CargoTotal.ToString("#0.00") + "</cbc:BaseAmount>");
                    xmlCab.Append("</cac:AllowanceCharge>");
                }
                #endregion
                /* FIN TAG <cac:AllowanceCharge> */
                /* INI TAG <cac:TaxTotal>  -- Impuesto por Detalle */
                xmlCab.Append("<cac:TaxTotal>");
                /* IGV Total del Item */
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                /* INI TAG <cac:TaxSubtotal>  ISC */
                if (factura.TotalVentaOpGratuitas > 0 || factura.TotalVentaOpInafectas > 0)
                {
                    /*                    xmlCab.Append("<cac:TaxSubtotal>");
                                        xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioUnitario.ToString("#0.00") + "</cbc:TaxableAmount>");
                                        xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");                
                                        xmlCab.Append("<cac:TaxCategory>");                
                                        xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                                        xmlCab.Append("<cbc:Percent>0.00</cbc:Percent>");
                                        xmlCab.Append("<cbc:TierRange>0</cbc:TierRange>");
                                        xmlCab.Append("<cac:TaxScheme>");
                                        xmlCab.Append("<cbc:ID>2000</cbc:ID>");
                                        xmlCab.Append("<cbc:Name>ISC</cbc:Name>");
                                        xmlCab.Append("<cbc:TaxTypeCode>EXC</cbc:TaxTypeCode>");
                                        xmlCab.Append("</cac:TaxScheme>");
                                        xmlCab.Append("</cac:TaxCategory>");
                                        xmlCab.Append("</cac:TaxSubtotal>");                   
                    */
                }

                /* FIN TAG <cac:TaxSubtotal>  ISC */
                /* INI TAG <cac:TaxSubtotal> */
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioUnitario.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                // DIFERENCIADO POR CONCEPTO VENTA
                xmlCab.Append("<cac:TaxCategory>");
                if (factura.TotalVentaOpGravadas > 0)
                {

                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");                    
                }

                if (factura.TotalVentaOpGratuitas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>0</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode listAgencyName=\"PE:SUNAT\" listName=\"Afectacion del IGV\" listURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07\" >" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }

                if (factura.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>0</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode listAgencyName=\"PE:SUNAT\" listName=\"Afectacion del IGV\" listURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07\" >" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }


                if (factura.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">E</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }
                /*                xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                                xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                                xmlCab.Append("<cac:TaxScheme>");   */
                xmlCab.Append("<cac:TaxScheme>");

                if (factura.TotalVentaOpGravadas > 0)
                {
                    xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                    xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                if (factura.TotalVentaOpGratuitas > 0)
                {
                    xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                    xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (factura.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID  schemeAgencyName=\"PE:SUNAT\" schemeID=\"UN/ECE 5153\" schemeName=\"Codigo de tributos\">9998</cbc:ID>");
                    xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (factura.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                    xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }

                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                /* FIN TAG <cac:TaxSubtotal> */
                xmlCab.Append("</cac:TaxTotal>");
                /* FIN TAG <cac:TaxTotal> */
                /* INI TAG <cac:Item> */
                xmlCab.Append("<cac:Item>");
                xmlCab.Append("<cbc:Description><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(item.DescripcionProducto) + "]]></cbc:Description>");
                xmlCab.Append("<cac:SellersItemIdentification><cbc:ID>" + item.CodigoProducto + "</cbc:ID></cac:SellersItemIdentification>");
                xmlCab.Append("</cac:Item>");
                /* FIN TAG <cac:Item> */
                /* INI TAG <cac:Price>*/
                xmlCab.Append("<cac:Price>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioSinIGV.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("</cac:Price>");
                /* FIN TAG <cac:Price>*/
                xmlCab.Append("</cac:InvoiceLine>");
                /* FIN TAG <cac:InvoiceLine> */
            }
            /* FIN Detalle de Factura */
            /* FIN TAG <ext:UBLExtensions> */
            xmlCab.Append("</Invoice>");
            strXml.AppendLine(xmlCab.ToString());

            return strXml;
        }

        public StringBuilder ArmarXmlBoletaUBL21(Documento boleta, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding =\"UTF-8\"?>");
            xmlCab.Append("<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            /* INI TAG <ext:UBLExtensions> */
            xmlCab.Append("<ext:UBLExtensions><ext:UBLExtension><ext:ExtensionContent/></ext:UBLExtension></ext:UBLExtensions>");
            /* Version XML*/
            xmlCab.Append("<cbc:UBLVersionID>2.1</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>2.0</cbc:CustomizationID>");
            /* FIN Version XML */
            /* INI Numero Documento */
            xmlCab.Append("<cbc:ID>" + boleta.NumeroComprobante + "</cbc:ID>");
            /* Fin Numero Documento */
            /* INI Fecha y Hora del Documento */
            xmlCab.Append("<cbc:IssueDate>" + boleta.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cbc:IssueTime>" + boleta.HoraEmision + "</cbc:IssueTime>");
            /* FIN Fecha y Hora del Documento */
            /* INI Tipo Documento */
            xmlCab.Append("<cbc:InvoiceTypeCode listID=\"0101\""); // Tipo de Operacion Catalogo 51 - 0101 Venta Interna
            xmlCab.Append(" listAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append(" listName=\"Tipo de Documento\" ");
            xmlCab.Append(" listURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo01\">" + boleta.CodigoTipoComprobante + "</cbc:InvoiceTypeCode>");
            /* Fin Tipo Documento */
            /* INI monto en Letras */
            /* xmlCab.Append(" <cbc:Note languageLocaleID=\"1000\"><![CDATA["+ new Funciones().LimpiarCaracteresInvalidos(factura.MontoEnLetras) + "]]></cbc:Note>"); */
            xmlCab.Append(" <cbc:Note><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.MontoEnLetras) + "]]></cbc:Note>");
            /* FIN monto en Letras */
            /* INI Moneda */
            xmlCab.Append(" <cbc:DocumentCurrencyCode listID=\"ISO 4217 Alpha\" listName=\"Currency\" listAgencyName=\"United Nations Economic Commission for Europe\">");
            xmlCab.Append(boleta.CodigoTipoMoneda + "</cbc:DocumentCurrencyCode>");
            /* FIN Moneda */
            /* INI Cantidad de Items de la Factura */
            xmlCab.Append("<cbc:LineCountNumeric>" + boleta.DetalleDocumento.Count.ToString() + "</cbc:LineCountNumeric>");
            /* FIN Cantidad de Items de la Factura */
            /* INI Orden de compra Cliente */
            if (boleta.sOrdenCompra == "S")
            {
                xmlCab.Append("<cac:OrderReference><cbc:ID>" + boleta.nOrdenCompra + "</cbc:ID></cac:OrderReference>");
            }
            /* FIN Orden de compra Cliente */

            /* INI Documentos Adicionales (NO ANTICIPO)*/
            if (boleta.sDocAdicional == "S")
            {
                foreach (DocumentoAdicional da in boleta.DetalleAdicionales)
                {
                    xmlCab.Append("<cac:AdditionalDocumentReference>");
                    xmlCab.Append("<cbc:ID>" + da.NumeroDocumento + "</cbc:ID>");
                    xmlCab.Append("<cbc:DocumentTypeCode>" + da.TipoDocumento + "</cbc:DocumentTypeCode>");
                    xmlCab.Append("</cac:AdditionalDocumentReference>");
                }
            }
            /* FIN Documentos Adicionales (NO ANTICIPO)*/
            /* INI TAG Signature */
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append("<cbc:ID>IDSignKG</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + boleta.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            /* FIN TAG Signature */

            /* INI TAG "<cac:AccountingSupplierParty>" */
            xmlCab.Append("<cac:AccountingSupplierParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"" + boleta.TipoDocumentoEmisor + "\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + boleta.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyName> -- Nombre Comercial */
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            /* FIN TAG <cac:PartyName> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Emisor*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Emisor */
            xmlCab.Append("<cac:RegistrationAddress>");
            /*xmlCab.Append("<cbc:ID>" + factura.UbigeoEmisor + "</cbc:ID>");*/
            xmlCab.Append("<cbc:AddressTypeCode>0000</cbc:AddressTypeCode>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + boleta.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + boleta.ProvinciaEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + boleta.DepartamentoEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + boleta.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.DireccionEmisor) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + boleta.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity>*/
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingSupplierParty>");
            /* FIN TAG <cac:AccountingSupplierParty> */
            /* INI TAG <cac:AccountingCustomerParty> -- Datos Adquiriente " */
            xmlCab.Append("<cac:AccountingCustomerParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"" + boleta.TipoDocumentoAdquirente + "\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + boleta.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Adquiriente*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Adquiriente*/
            xmlCab.Append("<cac:RegistrationAddress>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(boleta.DireccionAdquirente) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>PE</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity> */
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingCustomerParty>");
            /* FIN TAG <cac:AccountingCustomerParty> */
            if (boleta.TotalDescuentos > 0)
            {
                /* INI TAG <cac:AllowanceCharge> */
                xmlCab.Append("<cac:AllowanceCharge>");
                xmlCab.Append("<cbc:ChargeIndicator>false</cbc:ChargeIndicator>");
                xmlCab.Append("<cbc:AllowanceChargeReasonCode>00</cbc:AllowanceChargeReasonCode>");
                xmlCab.Append("<cbc:MultiplierFactorNumeric>" + boleta.sFactorDescuento.ToString("#0.00") + "</cbc:MultiplierFactorNumeric>");
                xmlCab.Append("<cbc:Amount currencyID=\"PEN\">" + boleta.TotalDescuentos.ToString("#0.00") + "</cbc:Amount>");
                xmlCab.Append("<cbc:BaseAmount currencyID=\"PEN\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:BaseAmount>");
                xmlCab.Append("</cac:AllowanceCharge>");
                /* FIN TAG <cac:AllowanceCharge> */
            }
            /* INI TAG <cac:TaxTotal> */
            xmlCab.Append("<cac:TaxTotal>");
            xmlCab.Append("<cbc:TaxAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
            /* OPeraciones Gravadas */
            if (boleta.TotalVentaOpGravadas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                // xmlCab.Append("</cac:TaxTotal>");
            }

            if (boleta.TotalVentaOpGratuitas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #region Exonerados

            if (boleta.TotalVentaOpExoneradas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpExoneradas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion

            #region INAFECTAS

            if (boleta.TotalVentaOpInafectas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpInafectas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9998</cbc:ID>");
                xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion

            xmlCab.Append("</cac:TaxTotal>");

            /* FIN TAG <cac:TaxTotal> */
            /* INI TAG <cac:LegalMonetaryTotal> -- Totales */
            //30
            xmlCab.Append("<cac:LegalMonetaryTotal>");
            xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.ImporteBrutoComprobante.ToString("#0.00") + "</cbc:LineExtensionAmount>");
            //31
            xmlCab.Append((boleta.ImporteTotalComprobante > 0 ? "<cbc:TaxInclusiveAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxInclusiveAmount>" : String.Empty));
            /*xmlCab.Append("<cbc:TaxExclusiveAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxExclusiveAmount>");*/
            //32
            xmlCab.Append((boleta.TotalDescuentos > 0 ? "<cbc:AllowanceTotalAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalDescuentos.ToString("#0.00") + "</cbc:AllowanceTotalAmount>" : String.Empty));
            //33
            xmlCab.Append((boleta.OTRCTotalComprobante > 0 ? "<cbc:ChargeTotalAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.OTRCTotalComprobante.ToString("#0.00") + "</cbc:ChargeTotalAmount>" : String.Empty));
            //34 = 31-32+33 
            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + ((boleta.ImporteTotalComprobante > 0 ? boleta.ImporteTotalComprobante : boleta.ImporteBrutoComprobante) - boleta.TotalDescuentos + boleta.OTRCTotalComprobante).ToString("#0.00") + "</cbc:PayableAmount>");
            //xmlCab.Append("<cbc:PayableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PayableAmount>");
            xmlCab.Append("</cac:LegalMonetaryTotal>");
            /*            if (boleta.TotalVentaOpGravadas > 0)
                        {
                            xmlCab.Append("<cac:LegalMonetaryTotal>");
                            xmlCab.Append((boleta.TotalDescuentos > 0 ? "<cbc:AllowanceTotalAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalDescuentos.ToString("#0.00") + "</cbc:AllowanceTotalAmount>" : String.Empty));
                            xmlCab.Append((boleta.OTRCTotalComprobante > 0 ? "<cbc:ChargeTotalAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.OTRCTotalComprobante.ToString("#0.00") + "</cbc:ChargeTotalAmount>" : String.Empty));
                            xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.ImporteBrutoComprobante.ToString("#0.00") + "</cbc:LineExtensionAmount>");
                            xmlCab.Append("<cbc:TaxExclusiveAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxExclusiveAmount>");
                            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PayableAmount>");
                            xmlCab.Append("</cac:LegalMonetaryTotal>");
                        }
                        if (boleta.TotalVentaOpGratuitas > 0)
                        {
                            xmlCab.Append((boleta.TotalDescuentos > 0 ? "<cbc:AllowanceTotalAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:AllowanceTotalAmount>" : String.Empty));
                            xmlCab.Append((boleta.OTRCTotalComprobante > 0 ? "<cbc:ChargeTotalAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:ChargeTotalAmount>" : String.Empty));
                            xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:LineExtensionAmount>");
                            xmlCab.Append("<cbc:TaxExclusiveAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxExclusiveAmount>");
                            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + boleta.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:PayableAmount>");
                            xmlCab.Append("</cac:LegalMonetaryTotal>");
                        }
            */
            /* FIN TAG <cac:LegalMonetaryTotal>*/
            /* INI Detalle de Factura */
            foreach (DocumentoDetalle item in boleta.DetalleDocumento)
            {
                /* INI TAG <cac:InvoiceLine>  -- Detalle de Item de la factura */
                xmlCab.Append("<cac:InvoiceLine>");
                xmlCab.Append("<cbc:ID>" + item.NumeroItem + "</cbc:ID>");
                xmlCab.Append("<cbc:InvoicedQuantity unitCode=\"" + item.UnidadMedida + "\">" + item.Cantidad.ToString("#0") + "</cbc:InvoicedQuantity>");
                /* Total del Item sin IGV */
                xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.TotalItemSinIGV.ToString("#0.00") + "</cbc:LineExtensionAmount>");
                /* INI TAG <cac:PricingReference>*/
                xmlCab.Append("<cac:PricingReference>");
                /* INI TAG <cac:AlternativeConditionPrice> - */
                xmlCab.Append("<cac:AlternativeConditionPrice>");
                /* Precio Unitario incluyendo IGV */
                // xmlCab.Append("<cbc:PriceAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + item.ValorVentaxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + boleta.CodigoTipoMoneda + "\">" + item.ValorUnitarioxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceTypeCode>" + item.CodigoTipoPrecio + "</cbc:PriceTypeCode>");
                xmlCab.Append("</cac:AlternativeConditionPrice>");
                /* FIN TAG <cac:AlternativeConditionPrice> */
                xmlCab.Append("</cac:PricingReference>");
                /* FIN TAG <cac:PricingReference> */
                /* INI TAG <cac:AllowanceCharge> */
                if (item.DescuentoxItem > 0)
                {
                    xmlCab.Append("<cac:AllowanceCharge>");
                    xmlCab.Append("<cbc:ChargeIndicator>false</cbc:ChargeIndicator>");
                    xmlCab.Append("<cbc:AllowanceChargeReasonCode>00</cbc:AllowanceChargeReasonCode>");
                    xmlCab.Append("<cbc:MultiplierFactorNumeric>" + item.sFactorDescuento.ToString("#0.0000") + "</cbc:MultiplierFactorNumeric>");
                    xmlCab.Append("<cbc:Amount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.DescuentoxItem.ToString("#0.00") + "</cbc:Amount>");
                    xmlCab.Append("<cbc:BaseAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.DescuentoTotal.ToString("#0.00") + "</cbc:BaseAmount>");
                    xmlCab.Append("</cac:AllowanceCharge>");
                }
                #region CargoItem
                if (item.CargoxItem > 0)
                {
                    xmlCab.Append("<cac:AllowanceCharge>");
                    xmlCab.Append("<cbc:ChargeIndicator>true</cbc:ChargeIndicator>");
                    xmlCab.Append("<cbc:AllowanceChargeReasonCode>50</cbc:AllowanceChargeReasonCode>");
                    xmlCab.Append("<cbc:MultiplierFactorNumeric>" + item.sFactorCargo.ToString("#0.0000") + "</cbc:MultiplierFactorNumeric>");
                    xmlCab.Append("<cbc:Amount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.CargoxItem.ToString("#0.00") + "</cbc:Amount>");
                    xmlCab.Append("<cbc:BaseAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.CargoTotal.ToString("#0.00") + "</cbc:BaseAmount>");
                    xmlCab.Append("</cac:AllowanceCharge>");
                }
                #endregion
                /* FIN TAG <cac:AllowanceCharge> */

                /* INI TAG <cac:TaxTotal>  -- Impuesto por Detalle */
                xmlCab.Append("<cac:TaxTotal>");
                /* IGV Total del Item */
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");

                /* INI TAG <cac:TaxSubtotal>  ISC */
                if (boleta.TotalVentaOpGratuitas > 0 || boleta.TotalVentaOpInafectas > 0)
                {
                    /*
                    xmlCab.Append("<cac:TaxSubtotal>");
                    xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioUnitario.ToString("#0.00") + "</cbc:TaxableAmount>");
                    xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");                
                    xmlCab.Append("<cac:TaxCategory>");                
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>0.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TierRange>0</cbc:TierRange>");
                    xmlCab.Append("<cac:TaxScheme>");
                    xmlCab.Append("<cbc:ID>2000</cbc:ID>");
                    xmlCab.Append("<cbc:Name>ISC</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>EXC</cbc:TaxTypeCode>");
                    xmlCab.Append("</cac:TaxScheme>");
                    xmlCab.Append("</cac:TaxCategory>");
                    xmlCab.Append("</cac:TaxSubtotal>");                    
                    */
                }
                /* FIN TAG <cac:TaxSubtotal>  ISC */
                /* INI TAG <cac:TaxSubtotal> */
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioUnitario.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                if (boleta.TotalVentaOpGravadas > 0)
                {

                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");                    
                }
                if (boleta.TotalVentaOpGratuitas > 0 || boleta.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>0</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }
                if (boleta.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">E</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }
                #region Nousada
                /*                if (boleta.TotalVentaOpGravadas > 0)
                                {
                                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                                }
                                if (boleta.TotalVentaOpGratuitas > 0)
                                {
                                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                                }
                                xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                                xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                                xmlCab.Append("<cac:TaxScheme>");  */
                #endregion
                xmlCab.Append("<cac:TaxScheme>");
                if (boleta.TotalVentaOpGravadas > 0)
                {
                    xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                    xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                if (boleta.TotalVentaOpGratuitas > 0)
                {
                    xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                    xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (boleta.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID>9998</cbc:ID>");
                    xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (boleta.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                    xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                /* FIN TAG <cac:TaxSubtotal> */
                xmlCab.Append("</cac:TaxTotal>");
                /* FIN TAG <cac:TaxTotal> */
                /* INI TAG <cac:Item> */
                xmlCab.Append("<cac:Item>");
                xmlCab.Append("<cbc:Description><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(item.DescripcionProducto) + "]]></cbc:Description>");
                xmlCab.Append("<cac:SellersItemIdentification><cbc:ID>" + item.CodigoProducto + "</cbc:ID></cac:SellersItemIdentification>");
                xmlCab.Append("</cac:Item>");
                /* FIN TAG <cac:Item> */
                /* INI TAG <cac:Price>*/
                xmlCab.Append("<cac:Price>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioSinIGV.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("</cac:Price>");
                /* FIN TAG <cac:Price>*/
                xmlCab.Append("</cac:InvoiceLine>");
                /* FIN TAG <cac:InvoiceLine> */
            }
            /* FIN Detalle de Factura */
            /* FIN TAG <ext:UBLExtensions> */
            xmlCab.Append("</Invoice>");
            strXml.AppendLine(xmlCab.ToString());

            return strXml;
        }

        public StringBuilder ArmarXmlNotaCreditoUBL21(Documento notacredito, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding =\"UTF-8\"?>");

            xmlCab.Append("<CreditNote xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            /* INI TAG <ext:UBLExtensions> */
            xmlCab.Append("<ext:UBLExtensions><ext:UBLExtension><ext:ExtensionContent/></ext:UBLExtension></ext:UBLExtensions>");
            xmlCab.Append("<cbc:UBLVersionID>2.1</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>2.0</cbc:CustomizationID>");
            /* FIN Version XML */
            /* INI Numero Documento */
            xmlCab.Append("<cbc:ID>" + notacredito.SerieNumeroCorrelativo + "</cbc:ID>");
            /* Fin Numero Documento */
            /* INI Fecha y Hora del Documento */
            xmlCab.Append("<cbc:IssueDate>" + notacredito.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cbc:IssueTime>" + notacredito.HoraEmision + "</cbc:IssueTime>");
            xmlCab.Append("<cbc:DocumentCurrencyCode>" + notacredito.CodigoTipoMoneda + "</cbc:DocumentCurrencyCode>");
            /* FIN Fecha y Hora del Documento */
            /* INI <cac:DiscrepancyResponse> */
            xmlCab.Append("<cac:DiscrepancyResponse>" +
                          "<cbc:ReferenceID>" + notacredito.SerieNumeroDocAfectado + "</cbc:ReferenceID>" + //<!-- Serie y numero de documento afectado) -->
                          "<cbc:ResponseCode>" + notacredito.CodigoTipoNotaCredito + "</cbc:ResponseCode>" + //<!-- Codigo de tipo de nota de credito -->
                          "<cbc:Description>" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.MotivoSustentoDocumento) + "</cbc:Description>" + //<!-- Motivo o Sustento -->
                          "</cac:DiscrepancyResponse>");
            /* FIN <cac:DiscrepancyResponse> */
            /* INI <cac:BillingReference> */
            xmlCab.Append("<cac:BillingReference>" +
                            "<cac:InvoiceDocumentReference>" +
                            "<cbc:ID>" + notacredito.SerieNumeroDocAfectado + "</cbc:ID>" + // <!-- Serie y nuºmero del documento que modifica -->
                            "<cbc:IssueDate>" + notacredito.FechaEmisionDocAfectado + "</cbc:IssueDate>" +
                            "<cbc:DocumentTypeCode>" + notacredito.CodigoTipoDocAfectado + "</cbc:DocumentTypeCode>" + //<!-- Tipo de documento del documento que modifica -->
                            "</cac:InvoiceDocumentReference>" +
                          "</cac:BillingReference>");
            /* FIN <cac:BillingReference> */
            /* INI TAG Signature */
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append("<cbc:ID>IDSignKG</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + notacredito.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            /* FIN TAG Signature */
            /* INI TAG "<cac:AccountingSupplierParty>" */
            xmlCab.Append("<cac:AccountingSupplierParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"6\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + notacredito.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyName> -- Nombre Comercial */
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            /* FIN TAG <cac:PartyName> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Emisor*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Emisor */
            xmlCab.Append("<cac:RegistrationAddress>");
            /*xmlCab.Append("<cbc:ID>" + factura.UbigeoEmisor + "</cbc:ID>");*/
            xmlCab.Append("<cbc:AddressTypeCode>0000</cbc:AddressTypeCode>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + notacredito.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + notacredito.ProvinciaEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + notacredito.DepartamentoEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + notacredito.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.DireccionEmisor) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + notacredito.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity>*/
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingSupplierParty>");
            /* FIN TAG <cac:AccountingSupplierParty> */
            /* INI TAG <cac:AccountingCustomerParty> -- Datos Adquiriente " */
            xmlCab.Append("<cac:AccountingCustomerParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"" + notacredito.TipoDocumentoAdquirente + "\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + notacredito.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Adquiriente*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Adquiriente*/
            xmlCab.Append("<cac:RegistrationAddress>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notacredito.DireccionAdquirente) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>PE</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity> */
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingCustomerParty>");
            /* FIN TAG <cac:AccountingCustomerParty> */

            /* INI TAG <cac:TaxTotal> */
            xmlCab.Append("<cac:TaxTotal>");
            xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
            if (notacredito.TotalVentaOpGravadas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                // xmlCab.Append("</cac:TaxTotal>");
            }

            if (notacredito.TotalVentaOpGratuitas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #region Exonerados

            if (notacredito.TotalVentaOpExoneradas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.TotalVentaOpExoneradas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion

            #region INAFECTAS

            if (notacredito.TotalVentaOpInafectas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.TotalVentaOpInafectas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9998</cbc:ID>");
                xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion
            xmlCab.Append("</cac:TaxTotal>");

            /* OPeraciones Gravadas */
            //if ( factura.TotalVentaOpGravadas ) { }
            /*xmlCab.Append("<cac:TaxSubtotal>");
            xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
            xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
            xmlCab.Append("<cac:TaxCategory>");
            xmlCab.Append("<cac:TaxScheme>");
            xmlCab.Append("<cbc:ID>1000</cbc:ID>");
            xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
            xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
            xmlCab.Append("</cac:TaxScheme>");
            xmlCab.Append("</cac:TaxCategory>");
            xmlCab.Append("</cac:TaxSubtotal>");*/
            //xmlCab.Append("</cac:TaxTotal>");
            /* FIN TAG <cac:TaxTotal> */

            /* INI TAG <cac:LegalMonetaryTotal> -- Totales */
            xmlCab.Append("<cac:LegalMonetaryTotal>");
            xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.ImporteBrutoComprobante.ToString("#0.00") + "</cbc:LineExtensionAmount>");
            //31
            xmlCab.Append((notacredito.ImporteTotalComprobante > 0 ? "<cbc:TaxInclusiveAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxInclusiveAmount>" : String.Empty));
            /*xmlCab.Append("<cbc:TaxExclusiveAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxExclusiveAmount>");*/
            //32
            xmlCab.Append((notacredito.TotalDescuentos > 0 ? "<cbc:AllowanceTotalAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.TotalDescuentos.ToString("#0.00") + "</cbc:AllowanceTotalAmount>" : String.Empty));
            //33
            xmlCab.Append((notacredito.OTRCTotalComprobante > 0 ? "<cbc:ChargeTotalAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.OTRCTotalComprobante.ToString("#0.00") + "</cbc:ChargeTotalAmount>" : String.Empty));
            //34 = 31-32+33 
            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + ((notacredito.ImporteTotalComprobante > 0 ? notacredito.ImporteTotalComprobante : notacredito.ImporteBrutoComprobante) - notacredito.TotalDescuentos + notacredito.OTRCTotalComprobante).ToString("#0.00") + "</cbc:PayableAmount>");
            //xmlCab.Append("<cbc:PayableAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PayableAmount>");
            xmlCab.Append("</cac:LegalMonetaryTotal>");
            /*xmlCab.Append("<cac:LegalMonetaryTotal>");
            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + notacredito.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PayableAmount>");
            xmlCab.Append("</cac:LegalMonetaryTotal>"); */
            /* FIN TAG <cac:LegalMonetaryTotal>*/
            /* INI Detalle de Nota Credito */
            foreach (DocumentoDetalle item in notacredito.DetalleDocumento)
            {
                /* INI TAG <cac:CreditNoteLine>  -- Detalle de Item de la factura */
                xmlCab.Append("<cac:CreditNoteLine>");
                xmlCab.Append("<cbc:ID>" + item.NumeroItem + "</cbc:ID>");
                xmlCab.Append("<cbc:CreditedQuantity unitCode=\"" + item.UnidadMedida + "\">" + item.Cantidad.ToString("#0") + "</cbc:CreditedQuantity>");
                /* Total del Item sin IGV */
                xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.TotalItemSinIGV.ToString("#0.00") + "</cbc:LineExtensionAmount>");
                /* INI TAG <cac:PricingReference>*/
                xmlCab.Append("<cac:PricingReference>");
                /* INI TAG <cac:AlternativeConditionPrice> - */
                xmlCab.Append("<cac:AlternativeConditionPrice>");
                /* Precio Unitario incluyendo IGV */
                // xmlCab.Append("<cbc:PriceAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + item.ValorVentaxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + notacredito.CodigoTipoMoneda + "\">" + item.ValorUnitarioxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceTypeCode>" + item.CodigoTipoPrecio + "</cbc:PriceTypeCode>");
                xmlCab.Append("</cac:AlternativeConditionPrice>");
                /* FIN TAG <cac:AlternativeConditionPrice> */
                xmlCab.Append("</cac:PricingReference>");
                /* FIN TAG <cac:PricingReference>*/
                /* INI TAG <cac:TaxTotal>  -- Impuesto por Detalle */
                xmlCab.Append("<cac:TaxTotal>");
                /* IGV Total del Item */
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                /* INI TAG <cac:TaxSubtotal> */
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioUnitario.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                if (notacredito.TotalVentaOpGravadas > 0)
                {

                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");                    
                }
                if (notacredito.TotalVentaOpGratuitas > 0 || notacredito.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>0</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }
                if (notacredito.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">E</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");                
                }
                xmlCab.Append("<cac:TaxScheme>");
                if (notacredito.TotalVentaOpGravadas > 0)
                {
                    xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                    xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                if (notacredito.TotalVentaOpGratuitas > 0)
                {
                    xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                    xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (notacredito.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID>9998</cbc:ID>");
                    xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (notacredito.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                    xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                xmlCab.Append("</cac:TaxScheme>");
                /*xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");*/
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                /* FIN TAG <cac:TaxSubtotal> */
                xmlCab.Append("</cac:TaxTotal>");
                /* FIN TAG <cac:TaxTotal> */
                /* INI TAG <cac:Item> */
                xmlCab.Append("<cac:Item>");
                xmlCab.Append("<cbc:Description><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(item.DescripcionProducto) + "]]></cbc:Description>");
                xmlCab.Append("<cac:SellersItemIdentification><cbc:ID>" + item.CodigoProducto + "</cbc:ID></cac:SellersItemIdentification>");
                xmlCab.Append("</cac:Item>");
                /* FIN TAG <cac:Item> */
                /* INI TAG <cac:Price>*/
                xmlCab.Append("<cac:Price>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioSinIGV.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("</cac:Price>");
                /* FIN TAG <cac:Price>*/
                xmlCab.Append("</cac:CreditNoteLine>");
                /* FIN TAG <cac:CreditNoteLine> */
            }
            /* FIN Detalle de Nota Credito */
            /* FIN TAG <ext:UBLExtensions> */
            xmlCab.Append("</CreditNote>");
            strXml.AppendLine(xmlCab.ToString());
            return strXml;
        }

        public StringBuilder ArmarXmlNotaDebitoUBL21(Documento notadebito, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding =\"UTF-8\"?>");
            xmlCab.Append("<DebitNote xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            /* INI TAG <ext:UBLExtensions> */
            xmlCab.Append("<ext:UBLExtensions><ext:UBLExtension><ext:ExtensionContent/></ext:UBLExtension></ext:UBLExtensions>");
            /* INI TAG <ext:UBLExtension> */
            /* Version XML*/
            xmlCab.Append("<cbc:UBLVersionID>2.1</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>2.0</cbc:CustomizationID>");
            /* FIN Version XML */
            /* INI Numero Documento */
            xmlCab.Append("<cbc:ID>" + notadebito.SerieNumeroCorrelativo + "</cbc:ID>");
            /* Fin Numero Documento */
            /* INI Fecha y Hora del Documento */
            xmlCab.Append("<cbc:IssueDate>" + notadebito.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cbc:IssueTime>12:00:00</cbc:IssueTime>");
            xmlCab.Append("<cbc:DocumentCurrencyCode>" + notadebito.CodigoTipoMoneda + "</cbc:DocumentCurrencyCode>");
            /* FIN Fecha y Hora del Documento */
            /* INI <cac:DiscrepancyResponse> */
            xmlCab.Append("<cac:DiscrepancyResponse>" +
                          "<cbc:ReferenceID>" + notadebito.SerieNumeroDocAfectado + "</cbc:ReferenceID>" + //<!-- Serie y numero de documento afectado) -->
                          "<cbc:ResponseCode>" + notadebito.CodigoTipoNotaDebito + "</cbc:ResponseCode>" + //<!-- Codigo de tipo de nota de credito -->
                          "<cbc:Description>" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.MotivoSustentoDocumento) + "</cbc:Description>" + //<!-- Motivo o Sustento -->
                          "</cac:DiscrepancyResponse>");
            /* FIN <cac:DiscrepancyResponse> */
            /* INI <cac:BillingReference> */
            xmlCab.Append("<cac:BillingReference>" +
                            "<cac:InvoiceDocumentReference>" +
                            "<cbc:ID>" + notadebito.SerieNumeroDocAfectado + "</cbc:ID>" + // <!-- Serie y nuºmero del documento que modifica -->
                            "<cbc:IssueDate>" + notadebito.FechaEmisionDocAfectado + "</cbc:IssueDate>" +
                            "<cbc:DocumentTypeCode>" + notadebito.CodigoTipoDocAfectado + "</cbc:DocumentTypeCode>" + //<!-- Tipo de documento del documento que modifica -->
                            "</cac:InvoiceDocumentReference>" +
                          "</cac:BillingReference>");
            /* FIN <cac:BillingReference> */
            /* INI TAG Signature */
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append("<cbc:ID>IDSignKG</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + notadebito.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            /* FIN TAG Signature */
            /* INI TAG "<cac:AccountingSupplierParty>" */
            xmlCab.Append("<cac:AccountingSupplierParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"6\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + notadebito.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyName> -- Nombre Comercial */
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            /* FIN TAG <cac:PartyName> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Emisor*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Emisor */
            xmlCab.Append("<cac:RegistrationAddress>");
            /*xmlCab.Append("<cbc:ID>" + factura.UbigeoEmisor + "</cbc:ID>");*/
            xmlCab.Append("<cbc:AddressTypeCode>0000</cbc:AddressTypeCode>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + notadebito.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + notadebito.ProvinciaEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + notadebito.DepartamentoEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + notadebito.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.DireccionEmisor) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + notadebito.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity>*/
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingSupplierParty>");
            /* FIN TAG <cac:AccountingSupplierParty> */
            /* INI TAG <cac:AccountingCustomerParty> -- Datos Adquiriente " */
            xmlCab.Append("<cac:AccountingCustomerParty>");
            /*INI TAG <cac:Party>*/
            xmlCab.Append("<cac:Party>");
            /* INI TAG <cac:PartyIdentification> RUC y Tipo Documento*/
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"" + notadebito.TipoDocumentoAdquirente + "\" ");
            xmlCab.Append("schemeName=\"Documento de Identidad\" ");
            xmlCab.Append("schemeAgencyName=\"PE:SUNAT\" ");
            xmlCab.Append("schemeURI=\"urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo06\">" + notadebito.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            /* FIN TAG <cac:PartyIdentification> */
            /* INI TAG <cac:PartyLegalEntity> -- Razon Social Adquiriente*/
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            /* INI TAG <cac:RegistrationAddress>  -- Direccion Adquiriente*/
            xmlCab.Append("<cac:RegistrationAddress>");
            xmlCab.Append("<cac:AddressLine>");
            xmlCab.Append("<cbc:Line><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(notadebito.DireccionAdquirente) + "]]></cbc:Line>");
            xmlCab.Append("</cac:AddressLine>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>PE</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:RegistrationAddress>");
            /* FIN TAG <cac:RegistrationAddress> */
            xmlCab.Append("</cac:PartyLegalEntity>");
            /* FIN TAG <cac:PartyLegalEntity> */
            xmlCab.Append("</cac:Party>");
            /*FIN TAG <cac:Party>*/
            xmlCab.Append("</cac:AccountingCustomerParty>");
            /* FIN TAG <cac:AccountingCustomerParty> */
            /* INI TAG <cac:TaxTotal> */
            xmlCab.Append("<cac:TaxTotal>");
            xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
            /* OPeraciones Gravadas */
            //if ( factura.TotalVentaOpGravadas ) { }
            if (notadebito.TotalVentaOpGravadas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                // xmlCab.Append("</cac:TaxTotal>");
            }

            if (notadebito.TotalVentaOpGratuitas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.TotalVentaOpGravadas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #region Exonerados

            if (notadebito.TotalVentaOpExoneradas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.TotalVentaOpExoneradas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion

            #region INAFECTAS

            if (notadebito.TotalVentaOpInafectas > 0)
            {
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.TotalVentaOpInafectas.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.IGVTotalComprobante.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                xmlCab.Append("<cac:TaxScheme>");
                xmlCab.Append("<cbc:ID>9998</cbc:ID>");
                xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                xmlCab.Append("</cac:TaxScheme>");
                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                //xmlCab.Append("</cac:TaxTotal>");
            }
            #endregion
            xmlCab.Append("</cac:TaxTotal>");
            /* FIN TAG <cac:TaxTotal> */
            /* INI TAG <cac:RequestedMonetaryTotal> -- Totales */
            xmlCab.Append("<cac:RequestedMonetaryTotal>");
            xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.ImporteBrutoComprobante.ToString("#0.00") + "</cbc:LineExtensionAmount>");
            //31
            xmlCab.Append((notadebito.ImporteTotalComprobante > 0 ? "<cbc:TaxInclusiveAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxInclusiveAmount>" : String.Empty));
            /*xmlCab.Append("<cbc:TaxExclusiveAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + factura.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TaxExclusiveAmount>");*/
            //32
            xmlCab.Append((notadebito.TotalDescuentos > 0 ? "<cbc:AllowanceTotalAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.TotalDescuentos.ToString("#0.00") + "</cbc:AllowanceTotalAmount>" : String.Empty));
            //33
            xmlCab.Append((notadebito.OTRCTotalComprobante > 0 ? "<cbc:ChargeTotalAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.OTRCTotalComprobante.ToString("#0.00") + "</cbc:ChargeTotalAmount>" : String.Empty));
            //34 = 31-32+33 
            xmlCab.Append("<cbc:PayableAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + ((notadebito.ImporteTotalComprobante > 0 ? notadebito.ImporteTotalComprobante : notadebito.ImporteBrutoComprobante) - notadebito.TotalDescuentos + notadebito.OTRCTotalComprobante).ToString("#0.00") + "</cbc:PayableAmount>");
            /*xmlCab.Append("<cbc:PayableAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + notadebito.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PayableAmount>");*/
            xmlCab.Append("</cac:RequestedMonetaryTotal>");
            ///* FIN TAG <cac:RequestedMonetaryTotal>*/
            /* INI Detalle de Nota Debito */

            foreach (DocumentoDetalle item in notadebito.DetalleDocumento)
            {
                /* INI TAG <cac:CreditNoteLine>  -- Detalle de Item de la factura */
                xmlCab.Append("<cac:DebitNoteLine>");
                xmlCab.Append("<cbc:ID>" + item.NumeroItem + "</cbc:ID>");
                xmlCab.Append("<cbc:DebitedQuantity unitCode=\"" + item.UnidadMedida + "\">" + item.Cantidad.ToString("#0") + "</cbc:DebitedQuantity>");
                /* Total del Item sin IGV */
                xmlCab.Append("<cbc:LineExtensionAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.TotalItemSinIGV.ToString("#0.00") + "</cbc:LineExtensionAmount>");
                /* INI TAG <cac:PricingReference>*/
                xmlCab.Append("<cac:PricingReference>");
                /* INI TAG <cac:AlternativeConditionPrice> - */
                xmlCab.Append("<cac:AlternativeConditionPrice>");
                /* Precio Unitario incluyendo IGV */
                // xmlCab.Append("<cbc:PriceAmount currencyID=\"" + factura.CodigoTipoMoneda + "\">" + item.ValorVentaxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + notadebito.CodigoTipoMoneda + "\">" + item.ValorUnitarioxItem.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("<cbc:PriceTypeCode>" + item.CodigoTipoPrecio + "</cbc:PriceTypeCode>");
                xmlCab.Append("</cac:AlternativeConditionPrice>");
                /* FIN TAG <cac:AlternativeConditionPrice> */
                xmlCab.Append("</cac:PricingReference>");
                /* FIN TAG <cac:PricingReference>*/
                /* INI TAG <cac:TaxTotal>  -- Impuesto por Detalle */
                xmlCab.Append("<cac:TaxTotal>");
                /* IGV Total del Item */
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                /* INI TAG <cac:TaxSubtotal> */
                xmlCab.Append("<cac:TaxSubtotal>");
                xmlCab.Append("<cbc:TaxableAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioUnitario.ToString("#0.00") + "</cbc:TaxableAmount>");
                xmlCab.Append("<cbc:TaxAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.IGVxItem.ToString("#0.00") + "</cbc:TaxAmount>");
                xmlCab.Append("<cac:TaxCategory>");
                if (notadebito.TotalVentaOpGravadas > 0)
                {

                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");                    
                }
                if (notadebito.TotalVentaOpGratuitas > 0 || notadebito.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">O</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>0</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");
                }
                if (notadebito.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">E</cbc:ID>");
                    xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                    xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                    //xmlCab.Append("<cac:TaxScheme>");                
                }
                xmlCab.Append("<cac:TaxScheme>");
                if (notadebito.TotalVentaOpGravadas > 0)
                {
                    xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                    xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                if (notadebito.TotalVentaOpGratuitas > 0)
                {
                    xmlCab.Append("<cbc:ID>9996</cbc:ID>");
                    xmlCab.Append("<cbc:Name>GRA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (notadebito.TotalVentaOpInafectas > 0)
                {
                    xmlCab.Append("<cbc:ID>9998</cbc:ID>");
                    xmlCab.Append("<cbc:Name>INA</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>FRE</cbc:TaxTypeCode>");
                }
                if (notadebito.TotalVentaOpExoneradas > 0)
                {
                    xmlCab.Append("<cbc:ID>9997</cbc:ID>");
                    xmlCab.Append("<cbc:Name>EXO</cbc:Name>");
                    xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                }
                xmlCab.Append("</cac:TaxScheme>");
                /* xmlCab.Append("<cbc:ID schemeID=\"UN /ECE 5305\" schemeName=\"Tax Category Identifier\" schemeAgencyName=\"United Nations Economic Commission for Europe\">S</cbc:ID>");
                 xmlCab.Append("<cbc:Percent>18.00</cbc:Percent>");
                 xmlCab.Append("<cbc:TaxExemptionReasonCode>" + item.CodigoTipoIGV + "</cbc:TaxExemptionReasonCode>");
                 xmlCab.Append("<cac:TaxScheme>");
                 xmlCab.Append("<cbc:ID>1000</cbc:ID>");
                 xmlCab.Append("<cbc:Name>IGV</cbc:Name>");
                 xmlCab.Append("<cbc:TaxTypeCode>VAT</cbc:TaxTypeCode>");
                 xmlCab.Append("</cac:TaxScheme>");*/

                xmlCab.Append("</cac:TaxCategory>");
                xmlCab.Append("</cac:TaxSubtotal>");
                /* FIN TAG <cac:TaxSubtotal> */
                xmlCab.Append("</cac:TaxTotal>");
                /* FIN TAG <cac:TaxTotal> */
                /* INI TAG <cac:Item> */
                xmlCab.Append("<cac:Item>");
                xmlCab.Append("<cbc:Description><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(item.DescripcionProducto) + "]]></cbc:Description>");
                xmlCab.Append("<cac:SellersItemIdentification><cbc:ID>" + item.CodigoProducto + "</cbc:ID></cac:SellersItemIdentification>");
                xmlCab.Append("</cac:Item>");
                /* FIN TAG <cac:Item> */
                /* INI TAG <cac:Price>*/
                xmlCab.Append("<cac:Price>");
                xmlCab.Append("<cbc:PriceAmount currencyID=\"" + item.CodigoTipoMoneda + "\">" + item.PrecioSinIGV.ToString("#0.00") + "</cbc:PriceAmount>");
                xmlCab.Append("</cac:Price>");
                /* FIN TAG <cac:Price>*/
                xmlCab.Append("</cac:DebitNoteLine>");
                /* FIN TAG <cac:CreditNoteLine> */
            }
            /* FIN Detalle de Nota Debito */
            /* FIN TAG <ext:UBLExtensions> */
            xmlCab.Append("</DebitNote>");
            strXml.AppendLine(xmlCab.ToString());
            return strXml;
        }

        public StringBuilder ArmarXmlRetencion(Retencion retencion, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding =\"UTF-8\"?>");
            xmlCab.Append("<Retention xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Retention-1\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:sac=\"urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            /* INI TAG <ext:UBLExtensions> */
            xmlCab.Append("<ext:UBLExtensions><ext:UBLExtension><ext:ExtensionContent/></ext:UBLExtension></ext:UBLExtensions>");
            /* INI TAG <ext:UBLExtension> */
            /* Version XML*/
            xmlCab.Append("<cbc:UBLVersionID>2.0</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>1.0</cbc:CustomizationID>");
            /* FIN Version XML */
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append($"<cbc:ID>{retencion.RUCEmisor}</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + retencion.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            xmlCab.Append("<cbc:ID>" + retencion.SerieComprobante + "-" + retencion.NumeroComprobante.Trim() + "</cbc:ID>");
            xmlCab.Append("<cbc:IssueDate>" + retencion.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cbc:IssueTime>12:00:00</cbc:IssueTime>");
            xmlCab.Append("<cac:AgentParty>");
            xmlCab.Append(" <cac:PartyIdentification> ");
            xmlCab.Append("<cbc:ID schemeID=\"6\">" + retencion.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("<cac:PostalAddress>");
            xmlCab.Append("<cbc:ID>" + retencion.UbigeoEmisor + "</cbc:ID>");
            xmlCab.Append("<cbc:StreetName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.DireccionEmisor) + "]]></cbc:StreetName>");
            //xmlCab.Append("<cbc:CitySubdivisionName>" + retencion.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + retencion.DepartamentoEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + retencion.ProvinciaEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + retencion.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + retencion.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:PostalAddress>");
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            xmlCab.Append("</cac:PartyLegalEntity>");
            xmlCab.Append("</cac:AgentParty>");
            xmlCab.Append("<cac:ReceiverParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"6\">" + retencion.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            //xmlCab.Append("<cac:PartyName>");
            //xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialAdquirente) + "]]></cbc:Name>");
            //xmlCab.Append("</cac:PartyName>");
            //xmlCab.Append("<cac:PostalAddress>");
            //xmlCab.Append("<cbc:ID>" + retencion.UbigeoAdquirente + "</cbc:ID>");
            //xmlCab.Append("<cbc:StreetName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.DireccionAdquirente) + "]]></cbc:StreetName>");
            //xmlCab.Append("<cbc:CitySubdivisionName>" + retencion.UrbanizacionAdquiriente + "</cbc:CitySubdivisionName>");
            //xmlCab.Append("<cbc:CityName>" + retencion.DepartamentoAdquiriente + "</cbc:CityName>");
            //xmlCab.Append("<cbc:CountrySubentity>" + retencion.ProvinciaAdquiriente + "</cbc:CountrySubentity>");
            //xmlCab.Append("<cbc:District>" + retencion.DistritoAdquiriente + "</cbc:District>");
            //xmlCab.Append("<cac:Country>");
            //xmlCab.Append("<cbc:IdentificationCode>" + retencion.CodigoPaisAdquiriente + "</cbc:IdentificationCode>");
            //xmlCab.Append("</cac:Country>");
            //xmlCab.Append("</cac:PostalAddress>");
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            xmlCab.Append("</cac:PartyLegalEntity>");
            xmlCab.Append("</cac:ReceiverParty>");
            xmlCab.Append("<sac:SUNATRetentionSystemCode>" + retencion.CodigoRetencion + "</sac:SUNATRetentionSystemCode>");
            xmlCab.Append("<sac:SUNATRetentionPercent>" + retencion.PorcentajeRetencion.ToString("#0.00") + "</sac:SUNATRetentionPercent>");
            xmlCab.Append("<cbc:Note><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.Observaciones) + "]]></cbc:Note>");
            xmlCab.Append("<cbc:TotalInvoiceAmount currencyID=\"" + retencion.CodigoTipoMoneda + "\">" + retencion.MontoTotalRetencion.ToString("#0.00") + "</cbc:TotalInvoiceAmount>");
            xmlCab.Append("<sac:SUNATTotalPaid currencyID=\"" + retencion.CodigoTipoMoneda + "\">" + retencion.MontoTotalPago.ToString("#0.00") + "</sac:SUNATTotalPaid>");

            int contador = 1;

            foreach (DetalleRetencion item in retencion.DetalleRetencion)
            {
                xmlCab.Append("<sac:SUNATRetentionDocumentReference>");
                xmlCab.Append("<cbc:ID schemeID=\"" + item.CodigoTipoComprobante + "\">" + item.SerieComprobanteRef + "-" + item.NumeroComprobanteRef + "</cbc:ID>");
                xmlCab.Append("<cbc:IssueDate>" + item.FechaEmisionComprobante + "</cbc:IssueDate>");
                xmlCab.Append("<cbc:TotalInvoiceAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.ImporteTotalComprobante.ToString("#0.00") + "</cbc:TotalInvoiceAmount>");
                xmlCab.Append("<cac:Payment>");
                xmlCab.Append("<cbc:ID>" + contador.ToString() + "</cbc:ID>");
                xmlCab.Append("<cbc:PaidAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.ImporteTotalComprobante.ToString("#0.00") + "</cbc:PaidAmount>");
                xmlCab.Append("<cbc:PaidDate>" + item.FechaPago + "</cbc:PaidDate>");
                xmlCab.Append("</cac:Payment>");
                xmlCab.Append("<sac:SUNATRetentionInformation>");
                xmlCab.Append("<sac:SUNATRetentionAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.MontoRetencion.ToString("#0.00") + "</sac:SUNATRetentionAmount>");
                xmlCab.Append("<sac:SUNATRetentionDate>" + item.FechaRetencion + "</sac:SUNATRetentionDate>");
                xmlCab.Append("<sac:SUNATNetTotalPaid currencyID=\"" + item.MonedaComprobante + "\">" + item.MontoTotalPago.ToString("#0.00") + "</sac:SUNATNetTotalPaid>");
                xmlCab.Append("<cac:ExchangeRate>");
                xmlCab.Append("<cbc:SourceCurrencyCode>" + item.MonedaTCambioOrigen + "</cbc:SourceCurrencyCode>");
                xmlCab.Append("<cbc:TargetCurrencyCode>" + item.MonedaTCambioDestino + "</cbc:TargetCurrencyCode>");
                xmlCab.Append("<cbc:CalculationRate>" + item.TipoCambio.ToString("#0.000000") + "</cbc:CalculationRate>");
                xmlCab.Append("<cbc:Date>" + item.FechaTipoCambio + "</cbc:Date>");
                xmlCab.Append("</cac:ExchangeRate>");
                xmlCab.Append("</sac:SUNATRetentionInformation>");
                xmlCab.Append("</sac:SUNATRetentionDocumentReference>");
                contador++;
            }
            xmlCab.Append("</Retention>");
            strXml.AppendLine(xmlCab.ToString());
            return strXml;
        }

        public StringBuilder ArmarXmlBaja(Retencion retencion, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\" standalone=\"no\"?>");
            xmlCab.Append("<Retention");
            xmlCab.Append(" xmlns=\"urn:sunat:names:specification:ubl:peru:schema:xsd:Retention-1\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:sac=\"urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            xmlCab.Append("<ext:UBLExtensions>");
            xmlCab.Append("<ext:UBLExtension>");
            xmlCab.Append("<ext:ExtensionContent>");
            xmlCab.Append("</ext:ExtensionContent>");
            xmlCab.Append("</ext:UBLExtension>");
            xmlCab.Append("</ext:UBLExtensions>");
            xmlCab.Append("<cbc:UBLVersionID>2.0</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>1.0</cbc:CustomizationID>");
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append("<cbc:ID>IDSignKG</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + retencion.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            xmlCab.Append("<cbc:ID>" + retencion.SerieComprobante + "-" + retencion.NumeroComprobante.Trim() + "</cbc:ID>");
            xmlCab.Append("<cbc:IssueDate>" + retencion.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cac:AgentParty>");
            xmlCab.Append(" <cac:PartyIdentification> ");
            xmlCab.Append("<cbc:ID schemeID=\"6\">" + retencion.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name>" + retencion.RazonSocialEmisor + "</cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("<cac:PostalAddress>");
            xmlCab.Append("<cbc:ID>" + retencion.UbigeoEmisor + "</cbc:ID>");
            xmlCab.Append("<cbc:StreetName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.DireccionEmisor) + "]]></cbc:StreetName>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + retencion.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + retencion.DepartamentoEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + retencion.ProvinciaEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + retencion.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + retencion.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:PostalAddress>");
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            xmlCab.Append("</cac:PartyLegalEntity>");
            xmlCab.Append("</cac:AgentParty>");
            xmlCab.Append("<cac:ReceiverParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"6\">" + retencion.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialAdquirente) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("<cac:PostalAddress>");
            xmlCab.Append("<cbc:ID>" + retencion.UbigeoAdquirente + "</cbc:ID>");
            xmlCab.Append("<cbc:StreetName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.DireccionAdquirente) + "]]></cbc:StreetName>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + retencion.UrbanizacionAdquiriente + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + retencion.DepartamentoAdquiriente + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + retencion.ProvinciaAdquiriente + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + retencion.DistritoAdquiriente + "</cbc:District>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + retencion.CodigoPaisAdquiriente + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:PostalAddress>");
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            xmlCab.Append("</cac:PartyLegalEntity>");
            xmlCab.Append("</cac:ReceiverParty>");
            xmlCab.Append("<sac:SUNATRetentionSystemCode>" + retencion.CodigoRetencion + "</sac:SUNATRetentionSystemCode>");
            xmlCab.Append("<sac:SUNATRetentionPercent>" + retencion.PorcentajeRetencion.ToString() + "</sac:SUNATRetentionPercent>");
            xmlCab.Append("<cbc:Note>" + retencion.Observaciones + "</cbc:Note>");
            xmlCab.Append("<cbc:TotalInvoiceAmount currencyID=\"" + retencion.CodigoTipoMoneda + "\">" + retencion.MontoTotalRetencion.ToString() + "</cbc:TotalInvoiceAmount>");
            xmlCab.Append("<sac:SUNATTotalPaid currencyID=\"" + retencion.CodigoTipoMoneda + "\">" + retencion.MontoTotalPago + "</sac:SUNATTotalPaid>");

            int contador = 1;

            foreach (DetalleRetencion item in retencion.DetalleRetencion)
            {
                xmlCab.Append("<sac:SUNATRetentionDocumentReference>");
                xmlCab.Append("<cbc:ID schemeID=\"" + item.CodigoTipoComprobante + "\">" + item.SerieComprobanteRef + "-" + item.NumeroComprobanteRef + "</cbc:ID>");
                xmlCab.Append("<cbc:IssueDate>" + item.FechaEmisionComprobante + "</cbc:IssueDate>");
                xmlCab.Append("<cbc:TotalInvoiceAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.ImporteTotalComprobante.ToString() + "</cbc:TotalInvoiceAmount>");
                xmlCab.Append("<cac:Payment>");
                xmlCab.Append("<cbc:ID>" + contador.ToString() + "</cbc:ID>");
                xmlCab.Append("<cbc:PaidAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.ImporteTotalComprobante.ToString() + "</cbc:PaidAmount>");
                xmlCab.Append("<cbc:PaidDate>" + item.FechaPago + "</cbc:PaidDate>");
                xmlCab.Append("</cac:Payment>");
                xmlCab.Append("<sac:SUNATRetentionInformation>");
                xmlCab.Append("<sac:SUNATRetentionAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.MontoRetencion.ToString() + "</sac:SUNATRetentionAmount>");
                xmlCab.Append("<sac:SUNATRetentionDate>" + item.FechaRetencion + "</sac:SUNATRetentionDate>");
                xmlCab.Append("<sac:SUNATNetTotalPaid currencyID=\"" + item.MonedaComprobante + "\">" + item.MontoTotalPago.ToString() + "</sac:SUNATNetTotalPaid>");
                xmlCab.Append("<cac:ExchangeRate>");
                xmlCab.Append("<cbc:SourceCurrencyCode>" + item.MonedaTCambioOrigen + "</cbc:SourceCurrencyCode>");
                xmlCab.Append("<cbc:TargetCurrencyCode>" + item.MonedaTCambioDestino + "</cbc:TargetCurrencyCode>");
                xmlCab.Append("<cbc:CalculationRate>" + item.TipoCambio.ToString() + "</cbc:CalculationRate>");
                xmlCab.Append("<cbc:Date>" + item.FechaTipoCambio + "</cbc:Date>");
                xmlCab.Append("</cac:ExchangeRate>");
                xmlCab.Append("</sac:SUNATRetentionInformation>");
                xmlCab.Append("</sac:SUNATRetentionDocumentReference>");
                contador++;
            }
            xmlCab.Append("</Retention>");
            strXml.AppendLine(xmlCab.ToString());
            return strXml;
        }

        public StringBuilder ArmarXmlResumenDiario(Retencion retencion, AppSettings appSettings)
        {
            StringBuilder strXml = new StringBuilder();
            // Cabecera XML
            StringBuilder xmlCab = new StringBuilder();
            xmlCab.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\" standalone=\"no\"?>");
            xmlCab.Append("<Retention");
            xmlCab.Append(" xmlns=\"urn:sunat:names:specification:ubl:peru:schema:xsd:Retention-1\"");
            xmlCab.Append(" xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\"");
            xmlCab.Append(" xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\"");
            xmlCab.Append(" xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\"");
            xmlCab.Append(" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"");
            xmlCab.Append(" xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\"");
            xmlCab.Append(" xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\"");
            xmlCab.Append(" xmlns:sac=\"urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1\"");
            xmlCab.Append(" xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\"");
            xmlCab.Append(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            xmlCab.Append("<ext:UBLExtensions>");
            xmlCab.Append("<ext:UBLExtension>");
            xmlCab.Append("<ext:ExtensionContent>");
            xmlCab.Append("</ext:ExtensionContent>");
            xmlCab.Append("</ext:UBLExtension>");
            xmlCab.Append("</ext:UBLExtensions>");
            xmlCab.Append("<cbc:UBLVersionID>2.0</cbc:UBLVersionID>");
            xmlCab.Append("<cbc:CustomizationID>1.0</cbc:CustomizationID>");
            xmlCab.Append("<cac:Signature>");
            xmlCab.Append("<cbc:ID>IDSignKG</cbc:ID>");
            xmlCab.Append("<cac:SignatoryParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID>" + retencion.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.NombreComercialEmisor) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("</cac:SignatoryParty>");
            xmlCab.Append("<cac:DigitalSignatureAttachment>");
            xmlCab.Append("<cac:ExternalReference>");
            xmlCab.Append("<cbc:URI>SignatureSP</cbc:URI>");
            xmlCab.Append("</cac:ExternalReference>");
            xmlCab.Append("</cac:DigitalSignatureAttachment>");
            xmlCab.Append("</cac:Signature>");
            xmlCab.Append("<cbc:ID>" + retencion.SerieComprobante + "-" + retencion.NumeroComprobante.Trim() + "</cbc:ID>");
            xmlCab.Append("<cbc:IssueDate>" + retencion.FechaEmision + "</cbc:IssueDate>");
            xmlCab.Append("<cac:AgentParty>");
            xmlCab.Append(" <cac:PartyIdentification> ");
            xmlCab.Append("<cbc:ID schemeID=\"6\">" + retencion.RUCEmisor + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name>" + retencion.RazonSocialEmisor + "</cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("<cac:PostalAddress>");
            xmlCab.Append("<cbc:ID>" + retencion.UbigeoEmisor + "</cbc:ID>");
            xmlCab.Append("<cbc:StreetName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.DireccionEmisor) + "]]></cbc:StreetName>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + retencion.UrbanizacionEmisor + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + retencion.DepartamentoEmisor + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + retencion.ProvinciaEmisor + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + retencion.DistritoEmisor + "</cbc:District>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + retencion.CodigoPaisEmisor + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:PostalAddress>");
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialEmisor) + "]]></cbc:RegistrationName>");
            xmlCab.Append("</cac:PartyLegalEntity>");
            xmlCab.Append("</cac:AgentParty>");
            xmlCab.Append("<cac:ReceiverParty>");
            xmlCab.Append("<cac:PartyIdentification>");
            xmlCab.Append("<cbc:ID schemeID=\"6\">" + retencion.RUCAdquirente + "</cbc:ID>");
            xmlCab.Append("</cac:PartyIdentification>");
            xmlCab.Append("<cac:PartyName>");
            xmlCab.Append("<cbc:Name><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialAdquirente) + "]]></cbc:Name>");
            xmlCab.Append("</cac:PartyName>");
            xmlCab.Append("<cac:PostalAddress>");
            xmlCab.Append("<cbc:ID>" + retencion.UbigeoAdquirente + "</cbc:ID>");
            xmlCab.Append("<cbc:StreetName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.DireccionAdquirente) + "]]></cbc:StreetName>");
            xmlCab.Append("<cbc:CitySubdivisionName>" + retencion.UrbanizacionAdquiriente + "</cbc:CitySubdivisionName>");
            xmlCab.Append("<cbc:CityName>" + retencion.DepartamentoAdquiriente + "</cbc:CityName>");
            xmlCab.Append("<cbc:CountrySubentity>" + retencion.ProvinciaAdquiriente + "</cbc:CountrySubentity>");
            xmlCab.Append("<cbc:District>" + retencion.DistritoAdquiriente + "</cbc:District>");
            xmlCab.Append("<cac:Country>");
            xmlCab.Append("<cbc:IdentificationCode>" + retencion.CodigoPaisAdquiriente + "</cbc:IdentificationCode>");
            xmlCab.Append("</cac:Country>");
            xmlCab.Append("</cac:PostalAddress>");
            xmlCab.Append("<cac:PartyLegalEntity>");
            xmlCab.Append("<cbc:RegistrationName><![CDATA[" + new Funciones(appSettings).LimpiarCaracteresInvalidos(retencion.RazonSocialAdquirente) + "]]></cbc:RegistrationName>");
            xmlCab.Append("</cac:PartyLegalEntity>");
            xmlCab.Append("</cac:ReceiverParty>");
            xmlCab.Append("<sac:SUNATRetentionSystemCode>" + retencion.CodigoRetencion + "</sac:SUNATRetentionSystemCode>");
            xmlCab.Append("<sac:SUNATRetentionPercent>" + retencion.PorcentajeRetencion.ToString() + "</sac:SUNATRetentionPercent>");
            xmlCab.Append("<cbc:Note>" + retencion.Observaciones + "</cbc:Note>");
            xmlCab.Append("<cbc:TotalInvoiceAmount currencyID=\"" + retencion.CodigoTipoMoneda + "\">" + retencion.MontoTotalRetencion.ToString() + "</cbc:TotalInvoiceAmount>");
            xmlCab.Append("<sac:SUNATTotalPaid currencyID=\"" + retencion.CodigoTipoMoneda + "\">" + retencion.MontoTotalPago + "</sac:SUNATTotalPaid>");

            int contador = 1;

            foreach (DetalleRetencion item in retencion.DetalleRetencion)
            {
                xmlCab.Append("<sac:SUNATRetentionDocumentReference>");
                xmlCab.Append("<cbc:ID schemeID=\"" + item.CodigoTipoComprobante + "\">" + item.SerieComprobanteRef + "-" + item.NumeroComprobanteRef + "</cbc:ID>");
                xmlCab.Append("<cbc:IssueDate>" + item.FechaEmisionComprobante + "</cbc:IssueDate>");
                xmlCab.Append("<cbc:TotalInvoiceAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.ImporteTotalComprobante.ToString() + "</cbc:TotalInvoiceAmount>");
                xmlCab.Append("<cac:Payment>");
                xmlCab.Append("<cbc:ID>" + contador.ToString() + "</cbc:ID>");
                xmlCab.Append("<cbc:PaidAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.ImporteTotalComprobante.ToString() + "</cbc:PaidAmount>");
                xmlCab.Append("<cbc:PaidDate>" + item.FechaPago + "</cbc:PaidDate>");
                xmlCab.Append("</cac:Payment>");
                xmlCab.Append("<sac:SUNATRetentionInformation>");
                xmlCab.Append("<sac:SUNATRetentionAmount currencyID=\"" + item.MonedaComprobante + "\">" + item.MontoRetencion.ToString() + "</sac:SUNATRetentionAmount>");
                xmlCab.Append("<sac:SUNATRetentionDate>" + item.FechaRetencion + "</sac:SUNATRetentionDate>");
                xmlCab.Append("<sac:SUNATNetTotalPaid currencyID=\"" + item.MonedaComprobante + "\">" + item.MontoTotalPago.ToString() + "</sac:SUNATNetTotalPaid>");
                xmlCab.Append("<cac:ExchangeRate>");
                xmlCab.Append("<cbc:SourceCurrencyCode>" + item.MonedaTCambioOrigen + "</cbc:SourceCurrencyCode>");
                xmlCab.Append("<cbc:TargetCurrencyCode>" + item.MonedaTCambioDestino + "</cbc:TargetCurrencyCode>");
                xmlCab.Append("<cbc:CalculationRate>" + item.TipoCambio.ToString() + "</cbc:CalculationRate>");
                xmlCab.Append("<cbc:Date>" + item.FechaTipoCambio + "</cbc:Date>");
                xmlCab.Append("</cac:ExchangeRate>");
                xmlCab.Append("</sac:SUNATRetentionInformation>");
                xmlCab.Append("</sac:SUNATRetentionDocumentReference>");
                contador++;
            }
            xmlCab.Append("</Retention>");
            strXml.AppendLine(xmlCab.ToString());
            return strXml;
        }

    }
}
