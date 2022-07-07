using System;
using System.Collections.Generic;

namespace SiogaUtils
{

    public class StatusResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public string MessageType { get; set; }
        public StatusResponse()
        {
            Success = true;
            Messages = new List<string>();
        }
    }

    public class StatusApiResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public List<GenericMessage> Messages { get; set; }

        public StatusApiResponse()
        {
            Success = true;
            Messages = new List<GenericMessage>();
        }
    }

    public class GenericMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public GenericMessage(string type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
