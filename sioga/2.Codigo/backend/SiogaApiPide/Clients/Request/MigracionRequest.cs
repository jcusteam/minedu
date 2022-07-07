namespace SiogaApiPide.Clients.Request
{
    public class MigracionRequest
    {
        public string token { get; set; }
        public string id_sistema { get; set; }
        public string email_sistema { get; set; }
        public string usuario_sistema { get; set; }
        public string tipo_documento { get; set; }
        public string nro_documento { get; set; }
        public string id_servicio { get; set; }
        public string ip_usuario_sistema { get; set; }
    }
}
