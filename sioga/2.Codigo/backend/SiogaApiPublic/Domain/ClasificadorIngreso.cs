using System;

namespace SiogaApiPublic.Domain
{
    public class ClasificadorIngreso
    {
        public int ClasificadorIngresoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}