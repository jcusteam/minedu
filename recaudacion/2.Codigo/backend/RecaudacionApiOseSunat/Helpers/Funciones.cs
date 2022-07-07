using System;
using System.Text;
using System.Xml;
using System.IO;
using Ionic.Zip;
using System.Text.RegularExpressions;
using System.Globalization;

namespace RecaudacionApiOseSunat.Helpers
{
    public class Funciones
    {
        private readonly AppSettings _appSettings;

        public Funciones(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        public static string NombreMes(int mes)
        {
            string nombremes = String.Empty;

            switch (mes)
            {
                case 1:
                    nombremes = "ENERO";
                    break;
                case 2:
                    nombremes = "FEBRERO";
                    break;
                case 3:
                    nombremes = "MARZO";
                    break;
                case 4:
                    nombremes = "ABRIL";
                    break;
                case 5:
                    nombremes = "MAYO";
                    break;
                case 6:
                    nombremes = "JUNIO";
                    break;
                case 7:
                    nombremes = "JULIO";
                    break;
                case 8:
                    nombremes = "AGOSTO";
                    break;
                case 9:
                    nombremes = "SETIEMBRE";
                    break;
                case 10:
                    nombremes = "OCTUBRE";
                    break;
                case 11:
                    nombremes = "NOVIEMBRE";
                    break;
                case 12:
                    nombremes = "DICIEMBRE";
                    break;
            }
            return nombremes;
        }

        public byte[] GenerarXml(StringBuilder xml)
        {
            string encoding = _appSettings.Encoding;

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = Encoding.GetEncoding(encoding)
            };

            MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms, xmlWriterSettings))
            {
                writer.WriteRaw(xml.ToString());
            }

            return StreamToByteArray(ms);
        }

        private byte[] StreamToByteArray(Stream inputStream)
        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException();
            }

            if (inputStream.CanSeek)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
            }

            byte[] output = new byte[inputStream.Length];
            int bytesRead = inputStream.Read(output, 0, output.Length);
            return output;
        }

        public bool SaveData(byte[] Data, string filename)
        {
            BinaryWriter Writer = null;

            try
            {
                // Create a new stream to write to the file
                Writer = new BinaryWriter(File.OpenWrite(filename), Encoding.UTF8);

                // Writer raw data                
                Writer.Write(Data);
                Writer.Flush();
                Writer.Close();
            }
            catch (Exception)
            {
                //...
                return false;
            }
            finally
            {

            }

            return true;
        }

        public Boolean ComprimirArchivo(string filenameIn, string filenameOut)
        {
            Boolean result = false;

            try
            {
                byte[] bytedata = File.ReadAllBytes(filenameIn);

                using (var zip = new ZipFile())
                {
                    zip.AddEntry(Path.GetFileName(filenameIn), bytedata);

                    zip.Save(filenameOut);
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public Boolean ComprimirArchivo(byte[] bytedata, string filenameIn, string filenameOut)
        {
            Boolean result = false;

            try
            {
                using (var zip = new ZipFile())
                {
                    zip.AddEntry(filenameIn, bytedata);

                    zip.Save(filenameOut);
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public Boolean DescomprimirArchivo(string filenameIn, string filenameOut)
        {
            Boolean result = false;

            try
            {
                using (var zip = new ZipFile())
                {
                    zip.ExtractAll(filenameIn);
                    //zip.AddEntry(filenameIn, bytedata);

                    //zip.Save(filenameOut);
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public string LimpiarCaracteresInvalidos(string contenido)
        {
            string cadena = String.Empty;

            if (contenido != null && contenido.Length > 0)
                cadena = Regex.Replace(contenido, @"[^\w\s\-\+]", "");
            return cadena;
        }

        public string ToFormat(string cadena)
        {
            string original = cadena;

            if (cadena.Length > 0)
                return String.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:0,0.00}", Convert.ToDouble(cadena));

            return original;
        }

        public string AddCeros(string cadena)
        {
            string original = cadena;
            String[] caracteres = cadena.Split('.');

            if (caracteres.Length > 1)
            {
                string enteros = caracteres[0];
                string decimales = caracteres[1];
                original = enteros + "." + (decimales.Length < 2 ? decimales.PadRight(2, '0') : decimales);
            }
            else if (caracteres.Length == 1)
            {
                string enteros = caracteres[0];
                original = enteros + ".00";
            }

            return original;
        }


    }
}
