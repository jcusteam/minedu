using System;

namespace RecaudacionApiReporte.Domain
{
    public class Kardex
    {
        public int KardexId { get; set; }
        public string Documento { get; set; }
        public int AnioPecosa { get; set; }
        public int NumeroPecosa { get; set; }
        public DateTime Fecha { get; set; }
        public string EntradaDocumento { get; set; }
        public int EntradaDel { get; set; }
        public int EntradaAl { get; set; }
        public int EntradaTotal { get; set; }
        public string SalidaDocumento { get; set; }
        public string SalidaDocumentoNumero { get; set; }
        public int SalidaDel { get; set; }
        public int SalidaAl { get; set; }
        public int SalidaTotal { get; set; }
        public int Saldo { get; set; }
    }
}
