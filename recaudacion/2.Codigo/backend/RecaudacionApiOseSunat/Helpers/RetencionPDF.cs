using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using RecaudacionApiOseSunat.Domain;
using RecaudacionUtils;

namespace RecaudacionApiOseSunat.Helpers
{
    public class RetencionPDF
    {
        public void GenerarPDF(string filename, string pathServer, Retencion clsObj, AppSettings appSettings)
        {

            var document = new Document(PageSize.A4);
            var filestr = new FileStream(filename, FileMode.Create);
            var writer = PdfWriter.GetInstance(document, filestr);

            // Open the Document for writing
            document.Open();

            string fontpath = appSettings.RutaFonts;
            string isCodeQR = appSettings.IsCodeQR;
            string rutaImagen = appSettings.RutaImagen.Replace("\\", @"\\");

            //string numeroresolucion = clsObj.NumeroResolucionEmisor;
            int textAlignmentLeft = 0;
            int textAlignmentCenter = 2;
            int textAlignmentRight = 1;
            //decimal dc_cero = 0;
            byte[] bImage = File.ReadAllBytes(rutaImagen);

            FontFactory.RegisterDirectory(fontpath);

            document.NewPage();
            //las coordenadas x, y, w, h son en milimetros, para el caso del texto debes calcular el tamaño ancho yalto q tengra, si se desborda se cfreara una nueva linea
            // el height dependera de la fuente (tipo letra y tamaño) es importante para q haga el salto de linea cuando se requiera y para la posiicion correcta dentro del documento
            itextUtil.iImage(document, writer, 10, 10, 80, 15, 0, bImage);


            itextUtil.iTexto(document, writer, 10, 27, 103, 3, 0, textAlignmentLeft, "HELVETICA", 9, "Bold", "Normal", "Black", clsObj.RazonSocialEmisor);
            itextUtil.iTexto(document, writer, 10, 34, 103, 3, 0, textAlignmentLeft, "HELVETICA", 8, "Normal", "Normal", "Black", $"Dirección: {clsObj.DireccionEmisor}");
            itextUtil.iTexto(document, writer, 10, 38, 103, 3, 0, textAlignmentLeft, "HELVETICA", 8, "Normal", "Normal", "Black", $"Telf: {clsObj.TelefonoEmisor}");

            itextUtil.iRectangle(document, writer, 115, 10, 82, 34, 0, 0, 1);
            itextUtil.iTexto(document, writer, 115, 13, 82, 6, 0, textAlignmentCenter, "HELVETICA", 12, "Bold", "Normal", "BlacK", $"RUC: {clsObj.RUCEmisor}");
            itextUtil.iTexto(document, writer, 115, 21, 82, 6, 0, textAlignmentCenter, "HELVETICA", 12, "Bold", "Normal", "BlacK", "COMPROBANTE DE RETENCIÓN");
            itextUtil.iTexto(document, writer, 115, 30, 82, 6, 0, textAlignmentCenter, "HELVETICA", 12, "Bold", "Normal", "BlacK", $"{clsObj.SerieComprobante} - {clsObj.NumeroComprobante}");

            itextUtil.iRectangle(document, writer, 10, 50, 187, 26, 0, 4, 1);

            string fechaEmision = Convert.ToDateTime(clsObj.FechaEmision).Day.ToString("00") + "/" + Convert.ToDateTime(clsObj.FechaEmision).Month.ToString("00") + "/" + Convert.ToDateTime(clsObj.FechaEmision).Year;
            itextUtil.iTexto(document, writer, 15, 48, 18, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "Razón Social ");
            itextUtil.iTexto(document, writer, 15, 56, 18, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "RUC ");
            itextUtil.iTexto(document, writer, 15, 62, 18, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "Direccion");

            itextUtil.iTexto(document, writer, 34, 48, 83, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": {clsObj.RazonSocialAdquirente}");
            itextUtil.iTexto(document, writer, 34, 56, 83, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": {clsObj.RUCAdquirente}");
            itextUtil.iTexto(document, writer, 34, 62, 83, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": {clsObj.DireccionAdquirente}");

            itextUtil.iTexto(document, writer, 135, 48, 32, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "Fecha Emision");
            itextUtil.iTexto(document, writer, 135, 55, 32, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "Régimen");
            itextUtil.iTexto(document, writer, 135, 62, 32, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "Tasa");
            itextUtil.iTexto(document, writer, 135, 68, 32, 6, 0, textAlignmentLeft, "HELVETICA", 8, "Bold", "Normal", "BlacK", "Tipo Moneda");

            itextUtil.iTexto(document, writer, 160, 48, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": {fechaEmision}");
            itextUtil.iTexto(document, writer, 160, 55, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": 01");
            itextUtil.iTexto(document, writer, 160, 62, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": {clsObj.PorcentajeRetencion}%");
            itextUtil.iTexto(document, writer, 160, 68, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $": SOLES");


            //rectangulo solo cierra a los detalles
            itextUtil.iRectangle(document, writer, 10, 79, 187, 165, 0, 4, 1);
            // para hacer una linea es igual q un rectangulo solo con height 0
            itextUtil.iRectangle(document, writer, 10, 84, 187, 0, 0, 0, 0.5);
            // para hacer una linea es igual q un rectangulo solo con wicth 0
            itextUtil.iRectangle(document, writer, 35, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 65, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 95, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 125, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 150, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 175, 79, 0, 165, 0, 0, 0.25);

            // cabecera del detalle
            itextUtil.iTexto(document, writer, 15, 80, 17, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Bold", "Normal", "BlacK", "TIPO");
            itextUtil.iTexto(document, writer, 36, 80, 17, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Bold", "Normal", "BlacK", "NUMERO");
            itextUtil.iTexto(document, writer, 66, 80, 17, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Bold", "Normal", "BlacK", "FECHA");
            itextUtil.iTexto(document, writer, 96, 80, 17, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Bold", "Normal", "BlacK", "MONEDA");
            itextUtil.iTexto(document, writer, 132, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "TOTAL");
            itextUtil.iTexto(document, writer, 157, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "TOTAL RETENIDO");
            itextUtil.iTexto(document, writer, 179, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "TOTAL PAGO");

            // de aqui en adelante la posicion y dependera de la cantidad de items que tenga el detalle
            double y = 85;// inicio desde la ultima posicion y mas 3 osea 75
            foreach (DetalleRetencion item in clsObj.DetalleRetencion)
            {
                string tipoDocName = "";
                if (item.CodigoTipoComprobante == Definition.SUNAT_TIPO_COMPROBANTE_FACTURA)
                {
                    tipoDocName = "FACTURA";
                }
                var fechaEmisionComprobante = Helpers.FormatDateToString(item.FechaEmisionComprobante);
                int p;// p es cantidad de lineas q tendra la palabra si no calza en el ancho fijo
                p = itextUtil.iTexto(document, writer, 15, y, 17, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Normal", "Normal", "BlacK", $"{tipoDocName}");
                itextUtil.iTexto(document, writer, 36, y, 17, 3, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $"{item.SerieComprobanteRef}-{item.NumeroComprobanteRef}");
                itextUtil.iTexto(document, writer, 66, y, 17, 3, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $"{fechaEmisionComprobante}");
                itextUtil.iTexto(document, writer, 96, y, 17, 3, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", $"{item.MonedaComprobante}");
                itextUtil.iTexto(document, writer, 132, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.ImporteTotalComprobante.ToString()));
                itextUtil.iTexto(document, writer, 157, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.MontoRetencion.ToString()));
                itextUtil.iTexto(document, writer, 179, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.MontoTotalPago.ToString()));
                y = y + (5 * p);//3 = espacio de alto que ocupa la fuente de acuerdo al tamaño es muy importante el tamaño de la fuente
            }

            //rectangulo solo cierra a los detalles
            itextUtil.iRectangle(document, writer, 10, 245, 187, 30, 0, 4, 1);
            itextUtil.iRectangle(document, writer, 75, 245, 0, 30, 0, 0, 0.5);
            itextUtil.iRectangle(document, writer, 145, 245, 0, 30, 0, 0, 0.5);
            itextUtil.iRectangle(document, writer, 145, 255, 52, 0, 0, 0, 0.5);
            itextUtil.iRectangle(document, writer, 145, 265, 52, 0, 0, 0, 0.5);

            string strDataPDF = clsObj.RUCEmisor;//RUC
            strDataPDF += "|" + clsObj.TipoDocumentoEmisor; //|TIPO DE DOCUMENTO
            strDataPDF += "|" + clsObj.SerieComprobante; //|SERIE
            strDataPDF += "|" + clsObj.NumeroComprobante; //|NUMERO 
            strDataPDF += "|" + clsObj.MontoTotalRetencion.ToString(); //|MONTO TOTAL RETENIDO
            strDataPDF += "|" + clsObj.MontoTotalPago.ToString(); //|MONTO TOTAL PAGO
            strDataPDF += "|" + clsObj.FechaEmision; //|FECHAEMISION
            strDataPDF += "|" + clsObj.TipoDocumentoAdquirente; //|TIPO DOCUMENTO ADQUIRENTE
            strDataPDF += "|" + clsObj.RUCAdquirente; //|NUMERO DOCUMENTO ADQUIRENTE
            strDataPDF += "|" + clsObj.DigestValue; //|VALOR RESUMEN
            strDataPDF += "|" + clsObj.SignatureValue; //|VALOR DE LA FIRMA

            if (isCodeQR == Definition.CODE_QR_SI)
                itextUtil.BarCodePDF17(document, writer, 80, 250, 60, 20, 0, strDataPDF);

            itextUtil.iTexto(document, writer, 149, 248, 30, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", "TOTAL RETENIDO S/.");
            itextUtil.iTexto(document, writer, 149, 258, 30, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", "TOTAL PAGADO S/.");

            itextUtil.iTexto(document, writer, 180, 248, 16, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", new Funciones(appSettings).ToFormat(clsObj.MontoTotalRetencion.ToString()));
            itextUtil.iTexto(document, writer, 180, 258, 16, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", new Funciones(appSettings).ToFormat(clsObj.MontoTotalPago.ToString()));

            //iRectangle(document, writer, 3, y, 190, 30, 0);

            document.Close();
            filestr.Close();
            writer.Close();

            //Close the Document - this saves the document contents to the output stream
            if (document.IsOpen()) document.Close();

        }

    }
}
