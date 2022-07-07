using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Security.Cryptography.X509Certificates;

namespace RecaudacionApiOseSunat.Helpers
{
    public static class itextUtil
    {
        static double dPixelMM = 2.88;

        public static int iTexto(Document document, PdfWriter writer, double x, double y, double w, double h, int iAngle, int iTextAlignment,
                    string vFontFamily, double dFontSize, string vFontWeight, string vFontStyle, string vFontForeground, string vData)
        {
            if (string.IsNullOrEmpty(vData))
                vData = "";

            x = x * dPixelMM;
            y = y * dPixelMM;
            h = h * dPixelMM;
            w = w * dPixelMM;

            y = PageSize.A4.Height - y - h;//y=pos y del contenedor, h = height contenedor, hc = heigh del objeto

            PdfContentByte cb = writer.DirectContent;
            int iStyle = iTextSharp.text.Font.NORMAL;
            if ((vFontStyle != "Normal" && vFontWeight != "Normal")) iStyle = iTextSharp.text.Font.BOLDITALIC;
            if ((vFontStyle == "Normal" && vFontWeight != "Normal")) iStyle = iTextSharp.text.Font.BOLD;
            if ((vFontStyle != "Normal" && vFontWeight == "Normal")) iStyle = iTextSharp.text.Font.ITALIC;
            if ((vFontStyle == "Normal" && vFontWeight == "Normal")) iStyle = iTextSharp.text.Font.NORMAL;

            Font oFont;
            oFont = FontFactory.GetFont(vFontFamily, (float)dFontSize, iStyle, new BaseColor(System.Drawing.ColorTranslator.FromHtml(vFontForeground)));


            double dCon = dFontSize * 18 / 100;

            int iPDFTextAlig = 0;

            switch (iTextAlignment)
            {
                case 2://CENTER
                    iPDFTextAlig = PdfContentByte.ALIGN_CENTER;
                    break;
                case 0://lEFT
                    iPDFTextAlig = PdfContentByte.ALIGN_LEFT;
                    break;
                case 1://RIGHT
                    iPDFTextAlig = PdfContentByte.ALIGN_RIGHT;
                    break;
                default://DEFECTO LEFT
                    iPDFTextAlig = PdfContentByte.ALIGN_LEFT;
                    break;
            }
            #region grados
            //0 Grados
            if (iAngle == 0 && iTextAlignment == 2)//CENTER
            {
                y = y + 0 + dCon;
                x = x + (w / 2);
            }
            if (iAngle == 0 && iTextAlignment == 0)//lEFT
            {
                y = y + 0 + dCon;
                x = x + 0;
            }
            if (iAngle == 0 && iTextAlignment == 1)//RIGHT
            {
                y = y + 0 + dCon;
                x = x + w;
            }
            //180 Grados
            if (iAngle == 180 && iTextAlignment == 2)//CENTER
            {
                y = y + h - dCon;
                x = x + (w / 2);
            }
            if (iAngle == 180 && iTextAlignment == 0)//lEFT
            {
                y = y + h - dCon;
                x = x + w;
            }
            if (iAngle == 180 && iTextAlignment == 1)//RIGHT
            {
                y = y + h - dCon;
                x = x + 0;
            }
            //90 Grados
            if (iAngle == 90 && iTextAlignment == 2)//CENTER
            {
                y = y + (w / 2);
                x = x + h - dCon;
            }
            if (iAngle == 90 && iTextAlignment == 0)//lEFT
            {
                y = y + 0;
                x = x + h - dCon;
            }
            if (iAngle == 90 && iTextAlignment == 1)//RIGHT
            {
                y = y + w;
                x = x + h - dCon;
            }
            //270 Grados
            if (iAngle == 270 && iTextAlignment == 2)//CENTER
            {
                y = y + (w / 2);
                x = x + 0;
            }
            if (iAngle == 270 && iTextAlignment == 0)//lEFT
            {
                y = y + w;
                x = x + 0 + dCon;
            }
            if (iAngle == 270 && iTextAlignment == 1)//RIGHT
            {
                y = y + 0;
                x = x + 0 + dCon;
            }
            #endregion

            List<string> palabras = new List<string>();


            palabras = new List<string>();
            List<string> listTextos = new List<string>();

            foreach (string str in vData.Split(' '))
            {
                if (str.Trim() != string.Empty)
                {
                    listTextos.Add(str);
                }
            }


            while (listTextos.Count() > 0)
            {
                for (int i = listTextos.Count() - 1; i >= 0; i--)
                {
                    string vGeneratedText = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        vGeneratedText += listTextos[j] + " ";
                    }
                    double dtextWidth = vGeneratedText.Length;
                    //BaseFont.CreateFont().GetWidthPoint(vGeneratedText, dFontSize);
                    //GetTextWidth(vGeneratedText, dFontSize, vFontFamily, vFontStyle, vFontWeight);

                    if (dtextWidth <= w || listTextos.Count == 1)
                    {
                        palabras.Add(vGeneratedText.Trim());
                        for (int k = i; k >= 0; k--)
                        {
                            listTextos.RemoveAt(k);
                        }
                        break;
                    }
                }

                // aqui hacer la logica de salto de linea
            }

            if (palabras.Count > 1)
            {
                for (int i = 0; i < palabras.Count; i++)
                {

                    Phrase phrase = new Phrase(new Chunk(palabras[i], oFont));
                    ColumnText.ShowTextAligned(cb, (int)iPDFTextAlig, phrase, (float)x, (float)y, iAngle);


                    if (iAngle == 0)//CENTER
                    {
                        y -= h;
                    }
                    else if (iAngle == 180)//CENTER
                    {
                        y += h;
                    }
                    else if (iAngle == 90)//CENTER
                    {
                        x += h;
                    }
                    else if (iAngle == 270)//CENTER
                    {
                        x -= h;
                    }

                }
            }
            else
            {
                Phrase phrase = new Phrase(new Chunk(vData, oFont));
                ColumnText.ShowTextAligned(cb, (int)iPDFTextAlig, phrase, (float)x, (float)y, iAngle);
            }
            return palabras.Count;

        }

        public static void iImage(Document document, PdfWriter writer, double x, double y, double w, double h, int iAngle, byte[] bImage)
        {
            x = x * dPixelMM;
            y = y * dPixelMM;
            h = h * dPixelMM;
            w = w * dPixelMM;

            y = PageSize.A4.Height - y - h;//y=pos y del contenedor, h = height contenedor, hc = heigh del objeto
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bImage);
            img.ScaleAbsolute((float)w, (float)h);
            img.SetAbsolutePosition((float)x, (float)y);
            img.RotationDegrees = iAngle;
            document.Add(img);
        }

        public static void BarCodePDF17(Document document, PdfWriter writer, double x, double y, double w, double h, int iAngle, string vData)
        {
            x = x * dPixelMM;
            y = y * dPixelMM;
            h = h * dPixelMM;
            w = w * dPixelMM;

            y = PageSize.A4.Height - y - h;//y=pos y del contenedor, h = height contenedor, hc = heigh del objeto

            BarcodePdf417 pdf417 = new BarcodePdf417();
            pdf417.SetText(vData);
            string text = vData;
            iTextSharp.text.Image img = pdf417.GetImage();
            img.ScaleAbsolute((float)w, (float)h);
            img.SetAbsolutePosition((float)x, (float)y);
            img.RotationDegrees = iAngle;
            document.Add(img);
        }

        public static void iRectangle(Document document, PdfWriter writer, double x, double y, double w, double h, int iAngle, int r, double iborder, string vFontForeground = "#00000000")
        {
            x = x * dPixelMM;
            y = y * dPixelMM;
            h = h * dPixelMM;
            w = w * dPixelMM;

            y = PageSize.A4.Height - y - h;//y=pos y del contenedor, h = height contenedor, hc = heigh del objeto
            double hh = 0;
            double ww = 0;
            switch (iAngle)
            {
                case 0:
                case 180:
                    hh = h;
                    ww = w;
                    break;
                case 90:
                case 270:
                    hh = w;
                    ww = h;
                    break;
            }
            PdfContentByte cb = writer.DirectContent;
            cb.SetLineWidth((float)iborder);
            cb.RoundRectangle((float)x, (float)y, (float)ww, (float)hh, r);
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(vFontForeground);
            //cb.SetRGBColorStroke(c.R, c.G, c.B);
            cb.Stroke();
        }


        private static X509Certificate2 ObtenerCertificadorPorSubjectName(string subjectname)
        {
            X509Certificate2 certificate;
            foreach (var name in new[] { StoreName.My, StoreName.Root })
            {
                foreach (var location in new[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine })
                {
                    certificate = BuscarSubjectInStore(subjectname, name, location);
                    if (certificate != null)
                    {
                        return certificate;
                    }
                }
            }
            //certificate was not found
            throw new Exception(string.Format("The certificate with SubjectName {0} was not found",
                                               subjectname));
        }

        private static X509Certificate2 BuscarSubjectInStore(string subjectname,
                                                   StoreName name, StoreLocation location)
        {
            var certStore = new X509Store(name, location);
            certStore.Open(OpenFlags.ReadOnly);
            var certCollection = certStore.Certificates.Find(X509FindType.FindBySubjectName,
                                                             subjectname, false);
            certStore.Close();


            if (certCollection.Count > 0)
            {
                return certCollection[0];
            }
            else
            {
                return null;
            }
        }
    }
}
