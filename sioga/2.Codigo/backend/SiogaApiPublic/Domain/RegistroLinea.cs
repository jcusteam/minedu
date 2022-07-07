using System;
using System.Collections.Generic;

namespace SiogaApiPublic.Domain
{
    public class RegistroLinea
    {
        public int RegistroLineaId { get; set; }
        public int UnidadEjecutoraId { get; set; }
        public int CuentaCorrienteId { get; set; }
        public int BancoId { get; set; }
        public int ReciboIngresoId { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int TipoDocumentoId { get; set; }
        public string Numero { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int TipoReciboIngresoId { get; set; }
        public string NumeroDeposito { get; set; }
        public decimal ImporteDeposito { get; set; }
        public DateTime FechaDeposito { get; set; }
        public string NumeroOficio { get; set; }
        public string NumeroComprobantePago { get; set; }
        public string ExpedienteSiaf { get; set; }
        public string NumeroResolucion { get; set; }
        public string ExpedienteESinad { get; set; }
        public int? NumeroESinad { get; set; }
        public string ValidarDeposito { get; set; }
        public string Observacion { get; set; }
        public int Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
        public List<RegistroLineaDetalle> RegistroLineaDetalle { get; set; }
        public RegistroLinea()
        {
            RegistroLineaDetalle = new List<RegistroLineaDetalle>();
        }
    }


    public class RegistroLineaDetalle
    {
        public int RegistroLineaId { get; set; }
        public int ClasificadorIngresoId { get; set; }
        public decimal Importe { get; set; }
        public string Referencia { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}