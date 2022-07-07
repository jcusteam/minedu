namespace RecaudacionApiFileServer.Domain
{
    public class FileContent
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

}
