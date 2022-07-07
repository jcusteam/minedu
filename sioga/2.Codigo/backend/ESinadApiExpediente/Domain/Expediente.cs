using System;
namespace ESinadApiExpediente.Domain
{
    public class Expediente
    {
        public string NumeroExpediente { get; set; }
        public int Numero { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Remite { get; set; }
        public string OficinaOrigen { get; set; }
        public string Registrador { get; set; }
        public string TipoDocumento { get; set; }
        public int Anio { get; set; }
        public string Estado { get; set; }
    }
}