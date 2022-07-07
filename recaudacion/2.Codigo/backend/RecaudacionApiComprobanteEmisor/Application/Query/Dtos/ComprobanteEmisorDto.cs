using System;

namespace RecaudacionApiComprobanteEmisor.Application.Query.Dtos
{
    public class ComprobanteEmisorDto
    {
        public int ComprobanteEmisorId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public string Firmante { get; set; }
        public string NumeroRuc { get; set; }
        public string TipoDocumento { get; set; }
        public string NombreComercial { get; set; }
        public string RazonSocial { get; set; }
        public string Ubigeo { get; set; }
        public string Direccion { get; set; }
        public string Urbanizacion { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string CodigoPais { get; set; }
        public string Telefono { get; set; }
        public string DireccionAlternativa { get; set; }
        public string NumeroResolucion { get; set; }
        public string UsuarioOSE { get; set; }
        public string ClaveOSE { get; set; }
        public string CorreoEnvio { get; set; }
        public string CorreoClave { get; set; }
        public string ServerMail { get; set; }
        public string ServerPort { get; set; }
        public string NombreArchivoCer { get; set; }
        public string NombreArchivoKey { get; set; }
        public bool Estado { get; set; }
    }
}
