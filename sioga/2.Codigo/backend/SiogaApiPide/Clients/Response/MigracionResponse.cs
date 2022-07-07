namespace SiogaApiPide.Clients.Response
{
    public class MigracionResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public bool success { get; set; }
        public MigracionResponse()
        {
            success = true;
        }
    }
}
