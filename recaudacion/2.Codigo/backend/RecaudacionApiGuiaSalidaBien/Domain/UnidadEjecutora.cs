using System;

namespace RecaudacionApiGuiaSalidaBien.Domain
{
    public class UnidadEjecutora
    {
        public int UnidadEjecutoraId { get; set; }
        public string Secuencia { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string NumeroRuc { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public bool Estado { get; set; }
    }
}
