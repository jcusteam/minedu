using System;

namespace SiogaApiAuthorization.Domain
{
    public class Sesion
    {
        public DateTime FechaCaducidad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaSesion { get; set; }
    }

    public class SesionAuth
    {
        public DateTime FECHA_CADUCIDAD { get; set; }
        public DateTime FECHA_CREACION { get; set; }
        public DateTime FECHA_ULTIMA_SESION { get; set; }
    }
}