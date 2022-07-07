using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using RecaudacionApiOseSunat.Domain;
using RecaudacionUtils;

namespace RecaudacionApiOseSunat.Helpers
{
    public class BoletaPDF
    {
        public void GenerarPDF(string filename, string pathServer, Documento clsObj, AppSettings appSettings)
        {
            Document document = new Document(PageSize.A4);
            FileStream filestr = new FileStream(filename, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, filestr);
            document.Open();

            string fontpath = appSettings.RutaFonts;
            string isCodeQR = appSettings.IsCodeQR;
            string rutaImagen = appSettings.RutaImagen.Replace("\\", @"\\");
            int textAlignmentLeft = 0;
            int textAlignmentCenter = 2;
            int textAlignmentRight = 1;
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
            itextUtil.iTexto(document, writer, 115, 13, 82, 6, 0, textAlignmentCenter, "HELVETICA", 12, "Bold", "Normal", "BlacK", "RUC: " + clsObj.RUCEmisor);
            itextUtil.iTexto(document, writer, 115, 21, 82, 6, 0, textAlignmentCenter, "HELVETICA", 12, "Bold", "Normal", "BlacK", "BOLETA ELECTRONICA");
            itextUtil.iTexto(document, writer, 115, 30, 82, 6, 0, textAlignmentCenter, "HELVETICA", 12, "Bold", "Normal", "BlacK", "N° " + clsObj.NumeroComprobante);

            itextUtil.iRectangle(document, writer, 10, 50, 187, 26, 0, 4, 1);

            string fechatmp = Convert.ToDateTime(clsObj.FechaEmision).Day.ToString("00") + "/" + Convert.ToDateTime(clsObj.FechaEmision).Month.ToString("00") + "/" + Convert.ToDateTime(clsObj.FechaEmision).Year;
            itextUtil.iTexto(document, writer, 15, 48, 18, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "SEÑORES(ES)");
            itextUtil.iTexto(document, writer, 15, 55, 18, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "RUC/DOI");
            itextUtil.iTexto(document, writer, 15, 62, 18, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "DIRECCION");

            itextUtil.iTexto(document, writer, 35, 48, 83, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", clsObj.RazonSocialAdquirente);
            itextUtil.iTexto(document, writer, 35, 55, 83, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", clsObj.RUCAdquirente);
            itextUtil.iTexto(document, writer, 35, 62, 83, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", clsObj.DireccionAdquirente);

            itextUtil.iTexto(document, writer, 121, 48, 32, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "FECHA EMISION");
            itextUtil.iTexto(document, writer, 121, 55, 32, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "CONDICION DE PAGO");
            itextUtil.iTexto(document, writer, 121, 62, 32, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "G.R.");
            itextUtil.iTexto(document, writer, 121, 69, 32, 6, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", "ORDEN DE COMPRA");

            itextUtil.iTexto(document, writer, 158, 48, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", fechatmp);
            itextUtil.iTexto(document, writer, 158, 55, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", clsObj.CondicionPagoEmisor);
            itextUtil.iTexto(document, writer, 158, 62, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", clsObj.GuiaRemision);
            itextUtil.iTexto(document, writer, 158, 69, 24, 6, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", clsObj.nOrdenCompra);
            //x, y, w, h

            //x, y, w, h

            //rectangulo solo cierra a los detalles
            itextUtil.iRectangle(document, writer, 10, 79, 187, 165, 0, 4, 1);
            // para hacer una linea es igual q un rectangulo solo con height 0
            itextUtil.iRectangle(document, writer, 10, 84, 187, 0, 0, 0, 0.5);
            // para hacer una linea es igual q un rectangulo solo con wicth 0
            // para hacer una linea es igual q un rectangulo solo con wicth 0
            itextUtil.iRectangle(document, writer, 114, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 128, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 143, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 160, 79, 0, 165, 0, 0, 0.25);
            itextUtil.iRectangle(document, writer, 178, 79, 0, 165, 0, 0, 0.25);

            // cabecera del detalle
            itextUtil.iTexto(document, writer, 17, 80, 92, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Bold", "Normal", "BlacK", "DESCRIPCION");
            itextUtil.iTexto(document, writer, 115, 80, 30, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Bold", "Normal", "BlacK", " U. M.");
            itextUtil.iTexto(document, writer, 125, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "CANT");
            itextUtil.iTexto(document, writer, 142, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "PRECIO UNIT.");
            itextUtil.iTexto(document, writer, 160, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "DESC. IMPORTE");
            itextUtil.iTexto(document, writer, 179, 80, 17, 3, 0, textAlignmentRight, "HELVETICA", 6, "Bold", "Normal", "BlacK", "VAL. VTA. TOTAL");

            // de aqui en adelante la posicion y dependera de la cantidad de items que tenga el detalle
            double y = 85;// inicio desde la ultima posicion y mas 3 osea 75
            foreach (DocumentoDetalle item in clsObj.DetalleDocumento)
            {
                int p;// p es cantidad de lineas q tendra la palabra si no calza en el ancho fijo
                var descripcion = item.DescripcionProducto;
                if (item.DescripcionProducto.Length > 72)
                {
                    descripcion = $"{item.DescripcionProducto.Substring(0, 72)}...";
                }

                p = itextUtil.iTexto(document, writer, 17, y, 92, 3, 0, textAlignmentLeft, "HELVETICA", 6, "Normal", "Normal", "BlacK", descripcion);
                itextUtil.iTexto(document, writer, 115, y, 30, 3, 0, textAlignmentLeft, "HELVETICA", 7, "Normal", "Normal", "BlacK", "Unidad"); // item.UnidadMedida);
                itextUtil.iTexto(document, writer, 125, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.Cantidad.ToString()));
                itextUtil.iTexto(document, writer, 142, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.PrecioSinIGV.ToString()));
                itextUtil.iTexto(document, writer, 160, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.DescuentoTotal.ToString()));
                itextUtil.iTexto(document, writer, 179, y, 17, 3, 0, textAlignmentRight, "HELVETICA", 7, "Normal", "Normal", "BlacK", new Funciones(appSettings).ToFormat(item.ValorVentaxItem.ToString()));
                y = y + (5 * p);//3 = espacio de alto que ocupa la fuente de acuerdo al tamaño es muy importante el tamaño de la fuente
            }


            itextUtil.iTexto(document, writer, 13, 215, 160, 4, 0, textAlignmentLeft, "HELVETICA", 9, "Bold", "Normal", "BlacK", "SON : " + clsObj.MontoEnLetras.ToUpper());

            itextUtil.iTexto(document, writer, 13, 225, 160, 4, 0, textAlignmentLeft, "HELVETICA", 9, "Normal", "Normal", "BlacK", "Representación de la BOLETA VENTA ELECTRONICA");
            itextUtil.iTexto(document, writer, 13, 230, 160, 4, 0, textAlignmentLeft, "HELVETICA", 9, "Normal", "Normal", "BlacK", "Consulte su documento en ");
            itextUtil.iTexto(document, writer, 13, 235, 160, 4, 0, textAlignmentLeft, "HELVETICA", 9, "Normal", "Normal", "BlacK", "Autorizado mediante resolución ");
            //rectangulo solo cierra a los detalles
            itextUtil.iRectangle(document, writer, 10, 245, 187, 30, 0, 4, 1);
            itextUtil.iRectangle(document, writer, 75, 245, 0, 30, 0, 0, 0.5);
            itextUtil.iRectangle(document, writer, 145, 245, 0, 30, 0, 0, 0.5);
            itextUtil.iRectangle(document, writer, 145, 255, 52, 0, 0, 0, 0.5);
            itextUtil.iRectangle(document, writer, 145, 265, 52, 0, 0, 0, 0.5);

            string strDataPDF = clsObj.RUCEmisor;//RUC
            strDataPDF += "|" + clsObj.TipoDocumentoEmisor; //|TIPO DE DOCUMENTO
            strDataPDF += "|" + clsObj.NumeroComprobante.Split('-')[0]; //|SERIE
            strDataPDF += "|" + clsObj.NumeroComprobante.Split('-')[1]; //|NUMERO 
            strDataPDF += "|" + clsObj.IGVTotalComprobante.ToString(); //|MTO TOTAL IGV
            strDataPDF += "|" + clsObj.ImporteTotalComprobante.ToString(); //|MONTO TOTAL COMPROBANTE
            strDataPDF += "|" + clsObj.FechaEmision; //|FECHAEMISION
            strDataPDF += "|" + clsObj.TipoDocumentoAdquirente; //|TIPO DOCUMENTO ADQUIRENTE
            strDataPDF += "|" + clsObj.RUCAdquirente; //|NUMERO DOCUMENTO ADQUIRENTE
            strDataPDF += "|" + clsObj.DigestValue; //|VALOR RESUMEN
            strDataPDF += "|" + clsObj.SignatureValue; //|VALOR DE LA FIRMA

            if (isCodeQR == Definition.CODE_QR_SI)
                itextUtil.BarCodePDF17(document, writer, 80, 250, 60, 20, 0, strDataPDF);

            itextUtil.iTexto(document, writer, 149, 248, 30, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", "SUB TOTAL S/.");
            itextUtil.iTexto(document, writer, 149, 258, 30, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", "I.G.V. (" + clsObj.ValorIGV + "%) S/.");
            itextUtil.iTexto(document, writer, 149, 268, 30, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", "TOTAL S/.");

            itextUtil.iTexto(document, writer, 180, 248, 16, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", new Funciones(appSettings).ToFormat(clsObj.ImporteBrutoComprobante.ToString()));
            itextUtil.iTexto(document, writer, 180, 258, 16, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", new Funciones(appSettings).ToFormat(clsObj.IGVTotalComprobante.ToString()));
            itextUtil.iTexto(document, writer, 180, 268, 16, 4, 0, textAlignmentRight, "HELVETICA", 9, "Bold", "Normal", "BlacK", new Funciones(appSettings).ToFormat(clsObj.ImporteTotalComprobante.ToString()));

            //iRectangle(document, writer, 3, y, 190, 30, 0);
            document.Close();
            filestr.Close();
            writer.Close();

            //Close the Document - this saves the document contents to the output stream
            if (document.IsOpen()) document.Close();

        }
    }
}
