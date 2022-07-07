using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RecaudacionApiComprobanteEmisor.Domain
{
    [Table("COMPROBANTE_EMISOR")]
    public class ComprobanteEmisor
    {
        [Key]
        [Column("COMPROBANTE_EMISOR_ID")]
        public int ComprobanteEmisorId { get; set; }
        [Column("UNIDAD_EJECUTORA_ID")]
        public int UnidadEjecutoraId { get; set; }
        [Column("COMPROBANTE_EMISOR_FIRMANTE")]
        public string Firmante { get; set; }
        [Column("COMPROBANTE_EMISOR_RUC")]
        public string NumeroRuc { get; set; }
        [Column("COMPROBANTE_EMISOR_TIPO_DOCUMENTO")]
        public string TipoDocumento { get; set; }
        [Column("COMPROBANTE_EMISOR_NOMBRE_COMERCIAL")]
        public string NombreComercial { get; set; }
        [Column("COMPROBANTE_EMISOR_RAZON_SOCIAL")]
        public string RazonSocial { get; set; }
        [Column("COMPROBANTE_EMISOR_UBIGEO")]
        public string Ubigeo { get; set; }
        [Column("COMPROBANTE_EMISOR_DIRECCION")]
        public string Direccion { get; set; }
        [Column("COMPROBANTE_EMISOR_URBANIZACION")]
        public string Urbanizacion { get; set; }
        [Column("COMPROBANTE_EMISOR_DEPARTAMENTO")]
        public string Departamento { get; set; }
        [Column("COMPROBANTE_EMISOR_PROVINCIA")]
        public string Provincia { get; set; }
        [Column("COMPROBANTE_EMISOR_DISTRITO")]
        public string Distrito { get; set; }
        [Column("COMPROBANTE_EMISOR_CODIGO_PAIS")]
        public string CodigoPais { get; set; }
        [Column("COMPROBANTE_EMISOR_TELEFONO")]
        public string Telefono { get; set; }
        [Column("COMPROBANTE_EMISOR_DIRECCION_ALTERNATIVA")]
        public string DireccionAlternativa { get; set; }
        [Column("COMPROBANTE_EMISOR_NUMERO_RESOLUCION")]
        public string NumeroResolucion { get; set; }
        [Column("COMPROBANTE_EMISOR_USUARIO_OSE")]
        public string UsuarioOSE { get; set; }
        [Column("COMPROBANTE_EMISOR_CLAVE_OSE")]
        public string ClaveOSE { get; set; }
        [Column("COMPROBANTE_EMISOR_CORREO_ENVIO")]
        public string CorreoEnvio { get; set; }
        [Column("COMPROBANTE_EMISOR_CORREO_CLAVE")]
        public string CorreoClave { get; set; }
        [Column("COMPROBANTE_EMISOR_SERVER_MAIL")]
        public string ServerMail { get; set; }
        [Column("COMPROBANTE_EMISOR_SEVER_PORT")]
        public string ServerPort { get; set; }
        [Column("COMPROBANTE_EMISOR_NOMBRE_CERTIFICADO")]
        public string NombreArchivoCer { get; set; }
        [Column("COMPROBANTE_EMISOR_NOMBRE_KEY")]
        public string NombreArchivoKey { get; set; }
        [Column("COMPROBANTE_EMISOR_ESTADO")]
        public bool Estado { get; set; }
        [Column("USUARIO_CREADOR")]
        public string UsuarioCreador { get; set; }
        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; }
        [Column("USUARIO_MODIFICADOR")]
        public string UsuarioModificador { get; set; }
        [Column("FECHA_MODIFICACION")]
        public DateTime? FechaModificacion { get; set; }
    }

}
