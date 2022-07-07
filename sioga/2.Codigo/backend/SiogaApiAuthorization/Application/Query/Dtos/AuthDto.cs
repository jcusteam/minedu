namespace SiogaApiAuthorization.Application.Query.Dtos
{
    public class AuthDto
    {
        public string CodigoModulo { get; set; }
    }

    public class AuthMenuDto
    {
        public string CodigoModulo { get; set; }
    }

    public class AuthAccionDto
    {
        public string CodigoModulo { get; set; }
        public string CodigoMenu { get; set; }
    }
}
