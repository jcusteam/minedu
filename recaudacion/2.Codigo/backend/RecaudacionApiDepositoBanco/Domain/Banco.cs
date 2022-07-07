namespace RecaudacionApiDepositoBanco.Domain
{
    public class Banco
    {
        public int BancoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }
    }
}