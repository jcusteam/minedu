using System.Collections.Generic;
using SiogaApiPide.Domain;

namespace SiogaApiPide.Clients.Response
{
    public class ReniecResponse
    {
        public Reniec data { get; set; }
        public bool success { get; set; }
        public List<string> messages { get; set; }
        public List<string> code { get; set; }
        public ReniecResponse()
        {
            messages = new List<string>();
            code = new List<string>();
        }
    }
}
