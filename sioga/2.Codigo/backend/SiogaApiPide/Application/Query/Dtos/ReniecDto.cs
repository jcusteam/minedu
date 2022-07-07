namespace SiogaApiPide.Application.Query.Dtos
{
    public class ReniecDto
    {
        public string numeroDni { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombres { get; set; }
        public string nombreCompleto { get; set; }

        public string paisDomicilio { get; set; }
        public string departamentoDomicilio { get; set; }
        public string provinciaDomicilio { get; set; }
        public string distritoDomicilio { get; set; }

        public string codigoEstadoCivil { get; set; }
        public string descripcionSexo { get; set; }
        public string fechaNacimiento { get; set; }
        public string direccion { get; set; }
        public string domicilioApp { get; set; }

    }
}
