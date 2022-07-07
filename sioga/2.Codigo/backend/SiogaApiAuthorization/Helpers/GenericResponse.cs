using System.Collections.Generic;

namespace SiogaApiAuthorization.Helpers
{
    public class GenericResponse<T> : GenericResponse
    {
        public T Data { get; set; }
    }

    public class GenericResponse
    {
        public GenericResponse()
        {
            Messages = new List<GenericMessagePass>();
        }
        protected GenericResponse(string errorMessage) : this()
        {
            HasErrors = true;
            Messages.Add(new GenericMessagePass(GenericMessageType.Error, message: errorMessage));
        }

        public bool HasErrors { get; set; }

        public List<GenericMessagePass> Messages { get; set; }
    }


    public class GenericMessagePass
    {
        public GenericMessagePass(GenericMessageType type, string message, string code = null)
        {
            Type = type;
            Message = message;
            Code = code;
        }

        public GenericMessageType Type { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }
    }

    public enum GenericMessageType
    {
        Info = 1,
        Warning = 2,
        Error = 3,
    }
}
