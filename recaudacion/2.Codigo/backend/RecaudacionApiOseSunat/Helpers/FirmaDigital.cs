using System;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.IO;
using System.Security.Cryptography;

namespace RecaudacionApiOseSunat.Helpers
{
    public class FirmaDigital
    {
        private readonly AppSettings _appSettings;

        public FirmaDigital(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public String ObtenerFirma(string firmate)
        {
            String SubjectName = firmate;

            X509Certificate2 myCert = ObtenerCertificadoPorSubjectName(SubjectName);
            XmlDocument xmlDoc = new XmlDocument();
            string contosoProducts = "<contenedor></contenedor>";
            xmlDoc.LoadXml(contosoProducts);

            SignedXml signedxml = new SignedXml(xmlDoc);

            signedxml.SigningKey = myCert.PrivateKey;

            Reference reference = new Reference();
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            reference.Uri = "";

            XmlDsigEnvelopedSignatureTransform transformacionENV = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(transformacionENV);

            signedxml.AddReference(reference);

            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data keyData = new KeyInfoX509Data(myCert);
            keyData.AddSubjectName(myCert.SubjectName.Name);
            keyInfo.AddClause(keyData);

            signedxml.KeyInfo = keyInfo;

            signedxml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            signedxml.ComputeSignature();

            XmlElement signature = signedxml.GetXml();
            foreach (XmlNode node in signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
            {
                node.Prefix = "ds";
            }

            return signature.InnerXml;
        }

        private static X509Certificate2 ObtenerCertificadoPorSubjectName(string subjectname)
        {
            X509Certificate2 certificate;
            foreach (var name in new[] { StoreName.My, StoreName.Root })
            {
                foreach (var location in new[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine })
                {
                    certificate = BuscarSubjectName(subjectname, name, location);
                    if (certificate != null)
                    {
                        return certificate;
                    }
                }
            }
            throw new Exception(string.Format("El certificado con SubjectName {0} no ha sido encontrado ", subjectname));
        }

        private static X509Certificate2 BuscarSubjectName(string subjectname, StoreName name, StoreLocation location)
        {
            var certStore = new X509Store(name, location);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2 certificate = null;

            foreach (X509Certificate2 item in certStore.Certificates)
            {
                if (item.SubjectName.Name.Contains(subjectname))
                {
                    certificate = item;
                    break;
                }
            }

            certStore.Close();

            return certificate;
        }

        public void FirmarPDF(string filenameorigen, string filenamedestino, string firmante)
        {
            String SubjectName = firmante;

            X509Certificate2 cert = ObtenerCertificadoPorSubjectName(SubjectName);

            //Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
            //Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] {
            //cp.ReadCertificate(cert.RawData)};

            //IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-1");

            //PdfReader pdfReader = new PdfReader(filenameorigen);

            //FileStream signedPdf = new FileStream(filenamedestino, FileMode.Create);

            //PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0');
            //PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;

            ////System.Drawing.Bitmap firma = Resources.firma;

            ////System.Drawing.ImageConverter imgfirma = new System.Drawing.ImageConverter();
            ////byte[] bytefirma = (byte[])imgfirma.ConvertTo(firma, typeof(byte[]));

            ////signatureAppearance.SignatureGraphic = Image.GetInstance(bytefirma);
            //signatureAppearance.SetVisibleSignature(new Rectangle(100, 100, 300, 150), pdfReader.NumberOfPages, "Signature");
            //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;

            //MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
        }
        public void FirmarXml(string pathxml, String etiquetapadre, ref String SignatureValue, ref String DigestValue,
        string nombreCer, string nombreKey, string firmante)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(pathxml);
            String StrCert = _appSettings.RutaCert.Replace("\\", @"\\") + "/" + nombreCer;
            String StrFilekey = _appSettings.RutaKey.Replace("\\", @"\\") + "/" + nombreKey;
            String Strkey;

            //String SubjectName = firmante;

            X509Certificate2 myCert = new X509Certificate2(StrCert);
            using (TextReader tr = new StreamReader(StrFilekey))
            {
                Strkey = tr.ReadToEnd();
            }

            byte[] keyBuffer = Helpers.GetBytesFromPEM(Strkey, PemStringType.RsaPrivateKey);
            RSACryptoServiceProvider rsaKey = Cryptokey.DecodeRsaPrivateKey(keyBuffer);
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = rsaKey;

            Reference reference = new Reference();
            reference.Uri = "";
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            KeyInfo KeyInfo = new KeyInfo();
            X509Chain X509Chain = new X509Chain();
            X509Chain.Build(myCert);
            X509ChainElement local_element = X509Chain.ChainElements[0];
            KeyInfoX509Data x509Data = new KeyInfoX509Data(local_element.Certificate);
            String subjectName = local_element.Certificate.Subject;
            x509Data.AddSubjectName(subjectName);
            KeyInfo.AddClause(x509Data);

            signedXml.KeyInfo = KeyInfo;
            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            xmlDigitalSignature.Prefix = "ds";
            signedXml.ComputeSignature();
            foreach (XmlNode node in xmlDigitalSignature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
            {
                if (node.LocalName == "Signature")
                {
                    XmlAttribute newAtribute = xmlDoc.CreateAttribute("Id");
                    newAtribute.Value = "SignatureSP";
                    node.Attributes.Append(newAtribute);
                }
            }

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);

            string l_xpath = "";

            if (pathxml.Contains("-01-")) //factura
            {
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                l_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }
            if (pathxml.Contains("-03-")) //BOLETA
            {
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

                l_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }
            if (pathxml.Contains("-07-")) //nota de credito
            {
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                l_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }
            if (pathxml.Contains("-08-"))//nota de debito
            {
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                l_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }
            if (pathxml.Contains("-20-"))//retencion
            {
                nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Retention-1");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                l_xpath = "/tns:Retention/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }
            if (pathxml.Contains("RA")) // documento de baja
            {
                nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                l_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }
            if (pathxml.Contains("RC"))// documento de revision de boleta
            {
                nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1");

                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

                l_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
            }

            XmlNode counterSignature = xmlDoc.SelectSingleNode(l_xpath, nsMgr);
            counterSignature.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
            xmlDoc.Save(pathxml);

        }

    }
}
