namespace RecaudacionApiLiquidacion.Domain
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public int TipoDocumentoIdentidadId { get; set; }
        public string TipoDocumentoNombre { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public bool Estado { get; set; }
    }
}
